using EulynxLive.FieldElementSubsystems.Configuration;
using EulynxLive.FieldElementSubsystems.Interfaces;
using EulynxLive.Point.Proto;
using Grpc.Core;
using System.Net.WebSockets;
using System.Text;
using System.Collections.Generic;
using System.Text.Json;

using static Point.Services.Extensions.AbilityToMoveConvertion;

namespace EulynxLive.Point
{
    public class Point : BackgroundService
    {
        public bool AllPointMachinesCrucial { get; }

        private readonly ILogger<Point> _logger;
        private readonly Func<Task> _simulateTimeout;
        private readonly List<WebSocket> _webSockets;
        private readonly IPointToInterlockingConnection _connection;
        private readonly Random _random;
        private readonly bool _simulateRandomTimeouts;
        private bool _initialized;
        private PreventedPosition _preventedPosition;
        private readonly GenericPointState _pointState;
        public GenericPointState PointState { get { return _pointState; } }

        public Point(ILogger<Point> logger, IConfiguration configuration, IPointToInterlockingConnection connection, Func<Task> simulateTimeout)
        {
            _logger = logger;
            _simulateTimeout = simulateTimeout;

            var config = configuration.GetSection("PointSettings").Get<PointConfiguration>() ?? throw new Exception("No configuration provided");
            if (config.AllPointMachinesCrucial == null)
            {
                _logger.LogInformation("Assuming all point machines are crucial.");
            }
            _simulateRandomTimeouts = config.SimulateRandomTimeouts ?? false;
            AllPointMachinesCrucial = config.AllPointMachinesCrucial ?? false;

            _webSockets = new List<WebSocket>();
            _pointState = new GenericPointState()
            {
                DegradedPointPosition = RespectAllPointMachinesCrucial(GenericDegradedPointPosition.NotDegraded),
                PointPosition = GenericPointPosition.NoEndPosition,
            };
            _preventedPosition = PreventedPosition.None;
            _random = new Random();
            _connection = connection;
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

        public async Task SendGenericMessage(GenericSCIMessage message){
            _logger.LogInformation("Sending generic message: {}", message.Message);
            await _connection.SendGenericMessage(message.Message.ToByteArray());
        }

        public async Task SendAbilityToMoveMessage(AbilityToMoveMessage message){
            _logger.LogInformation("Ability to move message sent. Ability to move: {}", message.Ability);
            await _connection.SendAbilityToMoveMessage(message.Ability.ConvertToGenericAbilityToMove());
        }

        public async Task SendTimeoutMessage(){
            _logger.LogInformation("Timeout");
            await _connection.SendTimeoutMessage();
        }

        public async Task PreventEndPosition(PreventedPositionMessage message)
        {
            _logger.LogInformation("Preventing end position {}.", message.Position);
            
            // Action a Trailed command immediately, otherwise store the prevented position for the next command.
            if (message.Position == PreventedPosition.PreventTrailed)
            {
                SetPointState(GenericPointPosition.UnintendedPosition, RespectAllPointMachinesCrucial(GenericDegradedPointPosition.NotDegraded));
                await UpdateConnectedWebClients();
            } else
            {
                _preventedPosition = message.Position;
            }
        }

        private void SetPointState(GenericPointPosition pointPosition, GenericDegradedPointPosition degradedPointPosition)
        {
            _pointState.PointPosition = pointPosition;
            _pointState.DegradedPointPosition = degradedPointPosition;
        }

        private void UpdatePointState(GenericPointPosition commandedPointPosition)
        {
            GenericPointPosition newPointPosition = _pointState.PointPosition;
            GenericDegradedPointPosition newDegradedPointPosition = _pointState.DegradedPointPosition;
            HandlePreventedPointPosition(commandedPointPosition, ref newPointPosition, ref newDegradedPointPosition);
            SetPointState(newPointPosition, RespectAllPointMachinesCrucial(newDegradedPointPosition));
        }

        /// <summary>
        /// Sets the <c>ReportedPointPosition</c> to the latest final position (left/right).
        /// </summary>
        /// <returns></returns>
        public async Task PutInEndPosition()
        {

            if (_connection != null)
            {
                if (_pointState.PointPosition != GenericPointPosition.Right &&
                _pointState.PointPosition != GenericPointPosition.Left)
                {
                    var reportedDegradedPointPosition = RespectAllPointMachinesCrucial(GenericDegradedPointPosition.NotDegraded);

                    GenericPointPosition? finalPosition = _pointState.DegradedPointPosition switch
                    {
                        GenericDegradedPointPosition.DegradedRight => GenericPointPosition.Right,
                        GenericDegradedPointPosition.DegradedLeft => GenericPointPosition.Left,
                        GenericDegradedPointPosition.NotDegraded => null,
                        GenericDegradedPointPosition.NotApplicable => null,
                        _ => null,
                    };
                    if (finalPosition != null)
                    {
                        _logger.LogInformation("Putting point into end position {}.", finalPosition);
                        SetPointState(finalPosition.Value, reportedDegradedPointPosition);
                        await _connection.SendPointPosition(PointState);
                    }
                }
            }
            await UpdateConnectedWebClients();
        }

        private GenericDegradedPointPosition RespectAllPointMachinesCrucial(GenericDegradedPointPosition degradedPointPosition) => AllPointMachinesCrucial ? GenericDegradedPointPosition.NotApplicable : degradedPointPosition;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Main loop.
            while (true)
            {
                _logger.LogTrace("Connecting...");
                var metadata = new Metadata { { "rasta-id", _connection.Configuration.LocalRastaId.ToString() } };
                using var grpc = new GrpcConnection(metadata, _connection.Configuration.RemoteEndpoint, _connection.TimeoutToken);
                _connection.Connect(grpc);
                await Reset();
                try
                {
                    var success = await _connection.InitializeConnection(PointState, stoppingToken);
                    if (!success)
                    {
                        throw new Exception("Unable to initialize connection");
                    }
                    await UpdateConnectedWebClients();

                    while (true)
                    {
                        var commandedPointPosition = await _connection.ReceivePointPosition(stoppingToken);
                        if (commandedPointPosition == null)
                        {
                            break;
                        }

                        if ((commandedPointPosition == GenericPointPosition.Left && _pointState.PointPosition == GenericPointPosition.Left)
                            || (commandedPointPosition == GenericPointPosition.Right && _pointState.PointPosition == GenericPointPosition.Right))
                        {
                            _logger.LogInformation("Point is already in position {}.", commandedPointPosition);
                            await _connection.SendPointPosition(PointState);
                            continue;
                        }

                        SetPointState(GenericPointPosition.NoEndPosition, RespectAllPointMachinesCrucial(_pointState.DegradedPointPosition));

                        await UpdateConnectedWebClients();

                        // Simulate point movement
                        var transitioningTime = _random.Next(1, 5);
                        var transitioningTask = Task.Delay(transitioningTime * 1000, CancellationToken.None);
                        var pointMovementTimeout = 3 * 1000;


                        if (_simulateRandomTimeouts)
                        {
                            if (await Task.WhenAny(transitioningTask, Task.Delay(pointMovementTimeout, CancellationToken.None)) == transitioningTask)
                            {
                                // transition completed within timeout
                                UpdatePointState(commandedPointPosition.Value);
                                _logger.LogDebug("Moving to {}.", _pointState.PointPosition);

                                await UpdateConnectedWebClients();

                                _logger.LogInformation("End position reached.");
                                await _connection.SendPointPosition(_pointState);
                            }
                            else
                            {
                                // timeout
                                await SendTimeoutMessage();
                            }
                        }
                        else
                        {
                            await _simulateTimeout();
                            UpdatePointState(commandedPointPosition.Value);
                            _logger.LogDebug("Moving to {}.", _pointState.PointPosition);

                            await UpdateConnectedWebClients();

                            _logger.LogInformation("End position reached.");
                            await _connection.SendPointPosition(_pointState);
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

        private void HandlePreventedPointPosition(GenericPointPosition commandedPosition, ref GenericPointPosition pointPosition, ref GenericDegradedPointPosition degradedPointPosition)
        {
            pointPosition = commandedPosition;
            switch (_preventedPosition)
            {
                case PreventedPosition.PreventedLeft:
                    if (commandedPosition == GenericPointPosition.Left)
                    {
                        degradedPointPosition = GenericDegradedPointPosition.DegradedLeft;
                        pointPosition = GenericPointPosition.NoEndPosition;
                    }
                    break;
                case PreventedPosition.PreventedRight:
                    if (commandedPosition == GenericPointPosition.Right)
                    {
                        degradedPointPosition = GenericDegradedPointPosition.DegradedRight;
                        pointPosition = GenericPointPosition.NoEndPosition;
                    }
                    break;
                case PreventedPosition.PreventTrailed:
                    degradedPointPosition = GenericDegradedPointPosition.NotDegraded;
                    pointPosition = GenericPointPosition.UnintendedPosition;
                    break;
            }
            _preventedPosition = PreventedPosition.None;
        }

        private async Task Reset()
        {
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
                {GenericPointPosition.UnintendedPosition, "trailed"},
            };
            var options = new JsonSerializerOptions { WriteIndented = true };
            var serializedState = JsonSerializer.Serialize(new
            {
                initialized = _initialized,
                position = positions[_pointState.PointPosition]
            }, options);
            var serializedStateBytes = Encoding.UTF8.GetBytes(serializedState);
            await webSocket.SendAsync(serializedStateBytes, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
