using EulynxLive.FieldElementSubsystems.Configuration;
using EulynxLive.FieldElementSubsystems.Interfaces;
using EulynxLive.Point.Connections;
using EulynxLive.Point.Proto;
using Grpc.Core;
using System.Net.WebSockets;
using System.Text;
using System.Collections.Generic;
using System.Text.Json;

using static Point.Services.Extensions.AbilityToMoveConvertion;
using Point.Services.Extensions;

namespace EulynxLive.Point
{
    public record SimulatedPointState{
        public PreventedPosition PreventedPosition { get; set; }
        public GenericDegradedPointPosition DegradedPointPosition { get; set; }
        public Boolean SimulateTimeout { get; set; }
        public AbilityToMove AbilityToMove { get; set; }
    }
    public class Point : BackgroundService
    {
        public bool AllPointMachinesCrucial { get; }

        private readonly ILogger<Point> _logger;
        private readonly Func<Task> _simulateTimeout;
        private readonly List<WebSocket> _webSockets;
        private readonly IPointToInterlockingConnection _connection;
        private readonly Random _random;
        private bool _initialized;
        private SimulatedPointState _simulatedPointState;
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
            AllPointMachinesCrucial = config.AllPointMachinesCrucial ?? false;

            _webSockets = new List<WebSocket>();
            _pointState = new GenericPointState()
            {
                DegradedPointPosition = RespectAllPointMachinesCrucial(GenericDegradedPointPosition.NotDegraded),
                PointPosition = GenericPointPosition.NoEndPosition,
            };
            _simulatedPointState = new SimulatedPointState()
            {
                PreventedPosition = PreventedPosition.None,
                DegradedPointPosition = RespectAllPointMachinesCrucial(GenericDegradedPointPosition.NotDegraded),
                SimulateTimeout = false,
                AbilityToMove = AbilityToMove.AbleToMove,
            };
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

        public async Task SendSCIMessage(GenericSCIMessage message){
            _logger.LogInformation("Sending generic message: {}", message.Message);
            await _connection.SendGenericMessage(message.Message.ToByteArray());
        }

        private async Task SendTimeoutMessage(){
            _logger.LogInformation("Timeout");
            await _connection.SendTimeoutMessage();
        }

        /// <summary>
        /// Sets the timeout flag for the next command.
        /// </summary>
        public void EnableTimeout(){
            _logger.LogInformation("Timeout on next command enabled.");
            _simulatedPointState.SimulateTimeout = true;
        }

        /// <summary>
        /// Sets the ability to move for the next command.
        /// </summary>
        public void SetAbilityToMove(AbilityToMoveMessage abilityToMoveMessage){
            _logger.LogInformation("Setting ability to move to {}.", abilityToMoveMessage.Ability);
            _simulatedPointState.AbilityToMove = abilityToMoveMessage.Ability;
        }

        /// <summary>
        /// Puts the point into an unintended position immediately.
        /// </summary>
        public async Task PutIntoUnintendedPosition(SimulatedPositionMessage simulatedPositionMessage)
        {
            _logger.LogInformation("Putting point into unintended position.");
            SetPointState(GenericPointPosition.UnintendedPosition, RespectAllPointMachinesCrucial(simulatedPositionMessage.DegradedPosition.ConvertToDegradedPointPosition()));
            await UpdateConnectedWebClients();
        }

        /// <summary>
        /// Stores the prevented position and degraded point position for the next command.
        /// Actions a Trailed command immediately.
        /// </summary>
        /// <param name="simulatedPositionMessage"></param>
        /// <returns></returns>
        public void PreventEndPosition(SimulatedPositionMessage simulatedPositionMessage)
        {
            if ((simulatedPositionMessage.Position == PreventedPosition.PreventedLeft && simulatedPositionMessage.DegradedPosition == PointDegradedPosition.DegradedRight) ||
                (simulatedPositionMessage.Position == PreventedPosition.PreventedRight && simulatedPositionMessage.DegradedPosition == PointDegradedPosition.DegradedLeft) ||
                (simulatedPositionMessage.Position == PreventedPosition.PreventTrailed))
            {
                throw new InvalidOperationException($"Prevented position {simulatedPositionMessage.Position} and degraded position {simulatedPositionMessage.DegradedPosition} are not compatible.");
            }

            _simulatedPointState.PreventedPosition = simulatedPositionMessage.Position;
            _simulatedPointState.DegradedPointPosition = simulatedPositionMessage.DegradedPosition.ConvertToDegradedPointPosition();
            _logger.LogInformation("Preventing end position {} with degraded point position {} on next command.", _simulatedPointState.PreventedPosition, _simulatedPointState.DegradedPointPosition);
        }

        private void SetPointState(GenericPointPosition pointPosition, GenericDegradedPointPosition degradedPointPosition)
        {
            _pointState.PointPosition = pointPosition;
            _pointState.DegradedPointPosition = degradedPointPosition;
        }

        /// <summary>
        /// Updates the point state based on the commanded position and the currently prevented position.
        /// <param name="commandedPointPosition"></param>
        /// </summary>
        private void UpdatePointState(GenericPointPosition commandedPointPosition)
        {
            GenericPointPosition newPointPosition = _pointState.PointPosition;
            GenericDegradedPointPosition newDegradedPointPosition = _pointState.DegradedPointPosition;
            (newPointPosition, newDegradedPointPosition) = HandlePreventedPointPosition(commandedPointPosition);
            HandleAbilityToMove(ref newPointPosition, ref newDegradedPointPosition);
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
                    throw new InvalidCastException($"All point machines are crucial, ignoring degraded point position {degradedPointPosition}.");
                return GenericDegradedPointPosition.NotApplicable;
            } else
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
                var metadata = new Metadata { { "rasta-id", _connection.Configuration.LocalRastaId.ToString() } };
                using var grpc = new GrpcConnection(metadata, _connection.Configuration.RemoteEndpoint, stoppingToken);
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
                    _initialized = true;
                    while (_initialized)
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

                        if (_simulatedPointState.SimulateTimeout)
                        {
                            // timeout
                            await SendTimeoutMessage();
                            await _simulateTimeout();
                            _simulatedPointState.SimulateTimeout = false;
                        }
                        else
                        {
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

        /// <summary>
        /// Sets the pointPosition and degradedPointPosition based on the commanded position and the currently prevented position.
        /// </summary>
        /// <param name="commandedPosition"></param>
        /// <param name="pointPosition"></param>
        /// <param name="degradedPointPosition"></param>
        private Tuple<GenericPointPosition, GenericDegradedPointPosition> HandlePreventedPointPosition(GenericPointPosition commandedPosition)
        {
            GenericPointPosition pointPosition = _pointState.PointPosition;
            GenericDegradedPointPosition degradedPointPosition = _pointState.DegradedPointPosition;

            pointPosition = commandedPosition;
            switch (_simulatedPointState.PreventedPosition)
            {
                case PreventedPosition.PreventedLeft:
                    if (commandedPosition == GenericPointPosition.Left)
                    {
                        degradedPointPosition = RespectAllPointMachinesCrucial(_simulatedPointState.DegradedPointPosition);
                        pointPosition = GenericPointPosition.NoEndPosition;
                    }
                    break;
                case PreventedPosition.PreventedRight:
                    if (commandedPosition == GenericPointPosition.Right)
                    {
                        degradedPointPosition = RespectAllPointMachinesCrucial(_simulatedPointState.DegradedPointPosition);
                        pointPosition = GenericPointPosition.NoEndPosition;
                    }
                    break;
                case PreventedPosition.PreventTrailed:
                    degradedPointPosition = RespectAllPointMachinesCrucial(_simulatedPointState.DegradedPointPosition);
                    pointPosition = GenericPointPosition.UnintendedPosition;
                    break;
            }
            _simulatedPointState.PreventedPosition = PreventedPosition.None;
            _simulatedPointState.DegradedPointPosition = GenericDegradedPointPosition.NotDegraded;
            return new Tuple<GenericPointPosition, GenericDegradedPointPosition>(pointPosition, degradedPointPosition);
        }

        private void HandleAbilityToMove(ref GenericPointPosition pointPosition, ref GenericDegradedPointPosition degradedPointPosition)
        {
            switch (_simulatedPointState.AbilityToMove)
            {
                case AbilityToMove.AbleToMove:
                    break;
                case AbilityToMove.UnableToMove:
                case AbilityToMove.Undefined:
                    _logger.LogWarning("Point is unable to move.");
                    degradedPointPosition = RespectAllPointMachinesCrucial(_pointState.DegradedPointPosition);
                    pointPosition = _pointState.PointPosition;
                    break;
            }
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
                position = positions[_pointState.PointPosition]
            }, options);
            var serializedStateBytes = Encoding.UTF8.GetBytes(serializedState);
            await webSocket.SendAsync(serializedStateBytes, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
