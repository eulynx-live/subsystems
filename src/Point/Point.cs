using EulynxLive.FieldElementSubsystems.Configuration;
using EulynxLive.FieldElementSubsystems.Interfaces;
using EulynxLive.Point.Proto;
using Grpc.Core;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace EulynxLive.Point
{

    public class Point : BackgroundService
    {
        public bool AllPointMachinesCrucial { get; }
        public GenericPointState PointState { get; private set; }
        public IPointToInterlockingConnection Connection { get; }

        private readonly ILogger<Point> _logger;
        private readonly IConnectionProvider _connectionProvider;
        private readonly Func<Task> _simulateTimeout;
        private readonly PointConfiguration _config;
        private readonly List<WebSocket> _webSockets;

        private readonly Random _random;
        private bool _initialized;
        private SimulatedPointState _simulatedPointState;

        public Point(ILogger<Point> logger, IConfiguration configuration, IPointToInterlockingConnection connection, IConnectionProvider connectionProvider, Func<Task> simulateTimeout)
        {
            _logger = logger;
            _simulateTimeout = simulateTimeout;

            _config = configuration.GetSection("PointSettings").Get<PointConfiguration>() ?? throw new Exception("No configuration provided");
            AllPointMachinesCrucial = _config.AllPointMachinesCrucial;

            _webSockets = new List<WebSocket>();
            PointState = new GenericPointState
            (
                LastCommandedPointPosition: _config.InitialLastCommandedPointPosition,
                DegradedPointPosition: RespectAllPointMachinesCrucial(_config.InitialDegradedPointPosition),
                PointPosition: _config.InitialPointPosition,
                AbilityToMove: _config.InitialAbilityToMove
            );

            _simulatedPointState = new SimulatedPointState(
                PreventedPositionLeft: PreventedPosition.DoNotPrevent,
                PreventedPositionRight: PreventedPosition.DoNotPrevent,
                DegradedPositionLeft: false,
                DegradedPositionRight: false,
                SimulateTimeoutLeft: false,
                SimulateTimeoutRight: false
            );
            _random = new Random();
            Connection = connection;
            _connectionProvider = connectionProvider;

            if (PointState.LastCommandedPointPosition == null && PointState.PointPosition != GenericPointPosition.NoEndPosition)
            {
                throw new InvalidOperationException("If the last commanded point position is unknown, the position reported by the point machine must be NoEndPosition.");
            }

            if (_config.AllPointMachinesCrucial && PointState.DegradedPointPosition != GenericDegradedPointPosition.NotApplicable)
            {
                throw new InvalidOperationException("If all point machines are crucial, the degraded point position must be NotApplicable.");
            }
        }

        public async Task HandleWebSocket(WebSocket webSocket)
        {
            _webSockets.Add(webSocket);
            try
            {
                await UpdateWebClient(webSocket);

                while (true)
                {
                    byte[] messageBuffer = new byte[1024];
                    var buffer = new ArraySegment<byte>(messageBuffer);
                    var result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
                    if (result.CloseStatus.HasValue)
                    {
                        break;
                    }
                }
            }
            catch (WebSocketException)
            {
                // Do nothing, the WebSocket has died.
            }
            _webSockets.Remove(webSocket);
        }

        public async Task SendSciMessage(SciMessage message)
        {
            _logger.LogInformation("Sending SCI message: {}", message.Message);
            await Connection.SendSciMessage(message.Message.ToByteArray());
        }

        private async Task SendTimeoutMessage()
        {
            _logger.LogInformation("Timeout");
            await Connection.SendTimeoutMessage();
        }

        /// <summary>
        /// Sets the timeout flag for the next move left command.
        /// </summary>
        public void EnableTimeoutLeft()
        {
            _logger.LogInformation("Timeout on next move left command enabled.");
            _simulatedPointState = _simulatedPointState with
            {
                SimulateTimeoutLeft = true
            };
        }

        /// <summary>
        /// Sets the timeout flag for the next move right command.
        /// </summary>
        public void EnableTimeoutRight()
        {
            _logger.LogInformation("Timeout on next move right command enabled.");
            _simulatedPointState = _simulatedPointState with
            {
                SimulateTimeoutRight = true
            };
        }

        /// <summary>
        /// Sets the ability to move for the next command.
        /// </summary>
        public void SetAbilityToMove(AbilityToMoveMessage abilityToMoveMessage)
        {
            _logger.LogInformation("Setting ability to move to {}.", abilityToMoveMessage.Ability);
            PointState = PointState with
            {
                AbilityToMove = abilityToMoveMessage.Ability switch
                {
                    AbilityToMove.AbleToMove => GenericAbilityToMove.AbleToMove,
                    AbilityToMove.UnableToMove => GenericAbilityToMove.UnableToMove,
                    AbilityToMove.Undefined => GenericAbilityToMove.Unknown,
                    _ => throw new InvalidOperationException($"Unknown ability to move {abilityToMoveMessage.Ability}.")
                }
            };
        }

        /// <summary>
        /// Puts the point into an unintended position immediately.
        /// </summary>
        public async Task PutIntoUnintendedPosition(DegradedPositionMessage simulatedPositionMessage)
        {
            if (AllPointMachinesCrucial && simulatedPositionMessage.DegradedPosition)
            {
                throw new InvalidOperationException("All point machines are crucial, cannot set degraded position.");
            }

            if (PointState.PointPosition != GenericPointPosition.Left && PointState.PointPosition != GenericPointPosition.Right)
            {
                throw new InvalidOperationException($"Point is not in an end position, cannot put into unintended position.");
            }

            _logger.LogInformation("Transitioning point into unintended / trailed position.");
            var notDegradedPosition = AllPointMachinesCrucial ? GenericDegradedPointPosition.NotApplicable : GenericDegradedPointPosition.NotDegraded;
            var degradedPosition = PointState.PointPosition == GenericPointPosition.Left ? GenericDegradedPointPosition.DegradedLeft : GenericDegradedPointPosition.DegradedRight;
            SetPointState(GenericPointPosition.UnintendedPosition, simulatedPositionMessage.DegradedPosition ? degradedPosition : notDegradedPosition);
            await UpdateConnectedWebClients();
        }

        /// <summary>
        /// Stores the prevented position and degraded point position for the next move left command.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public void PreventLeftEndPosition(PreventedPositionMessage request)
        {
            if (request.DegradedPosition && AllPointMachinesCrucial)
            {
                throw new InvalidOperationException("All point machines are crucial, cannot set degraded position.");
            }

            _simulatedPointState = _simulatedPointState with {
                PreventedPositionLeft = request.Position,
                DegradedPositionLeft = request.DegradedPosition,
            };
        }

        /// <summary>
        /// Stores the prevented position and degraded point position for the next move right command.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public void PreventRightEndPosition(PreventedPositionMessage request)
        {
            if (request.DegradedPosition && AllPointMachinesCrucial)
            {
                throw new InvalidOperationException("All point machines are crucial, cannot set degraded position.");
            }

            _simulatedPointState = _simulatedPointState with {
                PreventedPositionRight = request.Position,
                DegradedPositionRight = request.DegradedPosition,
            };
        }

        private void SetPointState(GenericPointPosition pointPosition, GenericDegradedPointPosition degradedPointPosition)
        {
            PointState = PointState with
            {
                PointPosition = pointPosition,
                DegradedPointPosition = degradedPointPosition,
            };
        }

        /// <summary>
        /// Updates the point state based on the commanded position and the currently prevented position.
        /// <param name="commandedPointPosition"></param>
        /// </summary>
        private void UpdatePointState(GenericPointPosition commandedPointPosition)
        {
            var (newPointPosition, newDegradedPointPosition) = HandlePreventedPointPosition(commandedPointPosition);
            (newPointPosition, newDegradedPointPosition) = HandleAbilityToMove(newPointPosition, newDegradedPointPosition);
            SetPointState(newPointPosition, RespectAllPointMachinesCrucial(newDegradedPointPosition));
        }

        /// <summary>
        /// Checks if AllPointMachinesCrucial is set and returns NotApplicable if it is.
        /// </summary>
        /// <param name="degradedPointPosition"></param>
        /// <returns></returns>
        private GenericDegradedPointPosition RespectAllPointMachinesCrucial(GenericDegradedPointPosition degradedPointPosition)
        {
            if (AllPointMachinesCrucial)
            {
                if (degradedPointPosition != GenericDegradedPointPosition.NotDegraded && degradedPointPosition != GenericDegradedPointPosition.NotApplicable)
                    throw new InvalidOperationException($"All point machines are crucial, ignoring degraded point position {degradedPointPosition}.");
                return GenericDegradedPointPosition.NotApplicable;
            }
            else
            {
                return degradedPointPosition;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Main loop.
            while (true)
            {
                _logger.LogTrace("Connecting...");
                var conn = _connectionProvider.Connect(Connection.Configuration, stoppingToken);
                Connection.Connect(conn);
                await Reset();
                try
                {
                    var success = await Connection.InitializeConnection(PointState, stoppingToken);
                    if (!success)
                    {
                        throw new Exception("Unable to initialize connection");
                    }
                    await UpdateConnectedWebClients();
                    _initialized = true;
                    while (_initialized)
                    {
                        var commandedPointPosition = await Connection.ReceiveMovePointCommand(stoppingToken);
                        if (commandedPointPosition == null)
                        {
                            break;
                        }

                        if ((commandedPointPosition == GenericPointPosition.Left && PointState.PointPosition == GenericPointPosition.Left)
                            || (commandedPointPosition == GenericPointPosition.Right && PointState.PointPosition == GenericPointPosition.Right))
                        {
                            _logger.LogInformation("Point is already in position {}.", commandedPointPosition);
                            await Connection.SendPointPosition(PointState);
                            continue;
                        }

                        SetPointState(GenericPointPosition.NoEndPosition, RespectAllPointMachinesCrucial(PointState.DegradedPointPosition));

                        await UpdateConnectedWebClients();

                        // Simulate point movement
                        var transitioningTime = _random.Next(1, 5);
                        var transitioningTask = Task.Delay(transitioningTime * 1000, CancellationToken.None);

                        if (_simulatedPointState.SimulateTimeoutLeft && commandedPointPosition == GenericPointPosition.Left
                            || _simulatedPointState.SimulateTimeoutRight && commandedPointPosition == GenericPointPosition.Right)
                        {
                            // timeout
                            await SendTimeoutMessage();
                            await _simulateTimeout();
                        }
                        else
                        {
                            UpdatePointState(commandedPointPosition);
                            _logger.LogDebug("Moving to {}.", PointState.PointPosition);

                            await UpdateConnectedWebClients();

                            _logger.LogInformation("End position reached.");
                            await Connection.SendPointPosition(PointState);
                        }
                    }
                }
                catch (RpcException)
                {
                    _logger.LogWarning("Could not communicate with remote endpoint.");
                    await Reset();
                    await Task.Delay(1000, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    await Reset();
                    return;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Exception during simulation.");
                    await Reset();
                    await Task.Delay(1000, stoppingToken);
                }
            }
        }

        /// <summary>
        /// Sets the pointPosition and degradedPointPosition based on the commanded position and the currently prevented position.
        /// </summary>
        /// <param name="commandedPosition"></param>
        /// <param name="pointPosition"></param>
        /// <param name="degradedPointPosition"></param>
        private (GenericPointPosition PointPosition, GenericDegradedPointPosition DegradedPointPosition) HandlePreventedPointPosition(GenericPointPosition commandedPosition)
        {
            var notDegradedPosition = AllPointMachinesCrucial ? GenericDegradedPointPosition.NotApplicable : GenericDegradedPointPosition.NotDegraded;

            if (commandedPosition == GenericPointPosition.Left)
            {
                if (_simulatedPointState.PreventedPositionLeft == PreventedPosition.DoNotPrevent)
                {
                    return (commandedPosition, notDegradedPosition);
                }
                else
                {
                    if (_simulatedPointState.PreventedPositionLeft == PreventedPosition.SetUnintendedOrTrailed)
                    {
                        return (GenericPointPosition.UnintendedPosition, _simulatedPointState.DegradedPositionLeft ? GenericDegradedPointPosition.DegradedLeft : notDegradedPosition);
                    }
                    else if (_simulatedPointState.PreventedPositionLeft == PreventedPosition.SetNoEndPosition)
                    {
                        return (GenericPointPosition.NoEndPosition, _simulatedPointState.DegradedPositionLeft ? GenericDegradedPointPosition.DegradedLeft : notDegradedPosition);
                    }
                }
            }
            else if (commandedPosition == GenericPointPosition.Right)
            {
                if (_simulatedPointState.PreventedPositionRight == PreventedPosition.DoNotPrevent)
                {
                    return (commandedPosition, notDegradedPosition);
                }
                else
                {
                    if (_simulatedPointState.PreventedPositionRight == PreventedPosition.SetUnintendedOrTrailed)
                    {
                        return (GenericPointPosition.UnintendedPosition, _simulatedPointState.DegradedPositionRight ? GenericDegradedPointPosition.DegradedRight : notDegradedPosition);
                    }
                    else if (_simulatedPointState.PreventedPositionRight == PreventedPosition.SetNoEndPosition)
                    {
                        return (GenericPointPosition.NoEndPosition, _simulatedPointState.DegradedPositionRight ? GenericDegradedPointPosition.DegradedRight : notDegradedPosition);
                    }
                }
            }

            throw new ArgumentException("Invalid commanded position", nameof(commandedPosition));
        }

        private (GenericPointPosition PointPosition, GenericDegradedPointPosition DegradedPointPosition) HandleAbilityToMove(GenericPointPosition pointPosition, GenericDegradedPointPosition degradedPointPosition)
        {
            switch (PointState.AbilityToMove)
            {
                case GenericAbilityToMove.AbleToMove:
                    return (pointPosition, degradedPointPosition);
                case GenericAbilityToMove.UnableToMove:
                case GenericAbilityToMove.Unknown:
                    _logger.LogWarning("Point is unable to move.");
                    return (PointState.PointPosition, RespectAllPointMachinesCrucial(PointState.DegradedPointPosition));
            }

            throw new InvalidOperationException($"Unknown ability to move {PointState.AbilityToMove}.");
        }

        public async Task Reset()
        {
            _logger.LogInformation("Resetting point.");
            _initialized = false;
            await UpdateConnectedWebClients();
        }

        private async Task UpdateConnectedWebClients()
        {
            try
            {
                var tasks = _webSockets.Select(UpdateWebClient);
                await Task.WhenAll(tasks);
            }
            catch (Exception)
            {
                // Some client likely has an issue, ignore
            }
        }

        private async Task UpdateWebClient(WebSocket webSocket)
        {
            var positions = new Dictionary<GenericPointPosition, string> {
                {GenericPointPosition.Right, "right"},
                {GenericPointPosition.Left, "left"},
                {GenericPointPosition.NoEndPosition, "noEndPosition"},
                {GenericPointPosition.UnintendedPosition, "unintendedPosition"},
            };
            var options = new JsonSerializerOptions { WriteIndented = true };
            var serializedState = JsonSerializer.Serialize(new
            {
                initialized = _initialized,
                position = positions[PointState.PointPosition]
            }, options);
            var serializedStateBytes = Encoding.UTF8.GetBytes(serializedState);
            await webSocket.SendAsync(serializedStateBytes, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
