using EulynxLive.Point.Proto;
using Grpc.Core;
using IPointToInterlockingConnection = EulynxLive.Point.Interfaces.IPointToInterlockingConnection;
using PointPosition = EulynxLive.Point.Interfaces.IPointToInterlockingConnection.PointPosition;
using PointState = EulynxLive.Point.Interfaces.IPointToInterlockingConnection.PointState;
using DegradedPointPosition = EulynxLive.Point.Interfaces.IPointToInterlockingConnection.DegradedPointPosition;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace EulynxLive.Point
{
    public class Point : BackgroundService
    {
        public bool AllPointMachinesCrucial { get; }

        private readonly ILogger<Point> _logger;
        private readonly Func<Task> _simulateTimeout;
        private readonly List<WebSocket> _webSockets;
        private IPointToInterlockingConnection _connection;
        private readonly Random _random;
        private readonly bool _simulateRandomTimeouts;
        private bool _initialized;
        private readonly PointState _pointState;
        private PreventedPosition _preventedPosition;
        public PointState PointState { get { return _pointState; } }

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
            _pointState = new PointState()
            {
                PointPosition = PointPosition.NoEndposition,
                DegradedPointPosition = RespectAllPointMachinesCrucial(DegradedPointPosition.NotDegraded)
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

        public void PreventEndPosition(PreventedPositionMessage message)
        {
            _logger.LogInformation("Preventing end position {}.", message.Position);
            _preventedPosition = message.Position;
        }

        private void SetPointState(PointPosition pointPosition, DegradedPointPosition degradedPointPosition)
        {
            _pointState.PointPosition = pointPosition;
            _pointState.DegradedPointPosition = degradedPointPosition;
        }

        private void UpdatePointState(PointPosition commandedPointPosition)
        {   
            PointPosition newPointPosition = _pointState.PointPosition;
            DegradedPointPosition newDegradedPointPosition = _pointState.DegradedPointPosition;
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
                if (_pointState.PointPosition != PointPosition.Right &&
                _pointState.PointPosition != PointPosition.Left)
                {
                    var reportedDegradedPointPosition = RespectAllPointMachinesCrucial(DegradedPointPosition.NotDegraded);

                    PointPosition? finalPosition = _pointState.DegradedPointPosition switch
                    {
                        DegradedPointPosition.DegradedRight => PointPosition.Right,
                        DegradedPointPosition.DegradedLeft => PointPosition.Left,
                        DegradedPointPosition.NotDegraded => null,
                        DegradedPointPosition.NotApplicable => null,
                        _ => null,
                    };
                    if (finalPosition != null)
                    {
                        _logger.LogInformation("Putting point into end position {}.", finalPosition);
                        SetPointState((PointPosition)finalPosition, reportedDegradedPointPosition);
                        await _connection.SendPointPosition(PointState);
                    }
                }
            }
            await UpdateConnectedWebClients();
        }

        private DegradedPointPosition RespectAllPointMachinesCrucial(DegradedPointPosition degradedPointPosition) => AllPointMachinesCrucial ? DegradedPointPosition.NotApplicable : degradedPointPosition;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Main loop.
            while (true)
            {
                _connection.Connect();
                await Reset();
                try
                {
                    var success = await _connection.InitializeConnection(PointState, stoppingToken);
                    if (!success)
                    {
                        continue;
                    }
                    await UpdateConnectedWebClients();

                    while (true)
                    {
                        var commandedPointPosition = await _connection.ReceivePointPosition(stoppingToken);
                        if (commandedPointPosition == null)
                        {
                            break;
                        }

                        if ((commandedPointPosition == PointPosition.Left && _pointState.PointPosition == PointPosition.Left)
                            || (commandedPointPosition == PointPosition.Right && _pointState.PointPosition == PointPosition.Right))
                        {
                            _logger.LogInformation("Point is already in position {}.", commandedPointPosition);
                            await _connection.SendPointPosition(PointState);
                            continue;
                        }

                        SetPointState(PointPosition.NoEndposition, RespectAllPointMachinesCrucial(_pointState.DegradedPointPosition));

                        await UpdateConnectedWebClients();

                        // Simulate point movement
                        var transitioningTime = _random.Next(1, 5);
                        var transitioningTask = Task.Delay(transitioningTime * 1000);
                        var pointMovementTimeout = 3 * 1000;


                        if (_simulateRandomTimeouts)
                        {
                            if (await Task.WhenAny(transitioningTask, Task.Delay(pointMovementTimeout)) == transitioningTask)
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
                                _logger.LogInformation("Timeout");
                                await _connection.SendTimeoutMessage();
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

        private void HandlePreventedPointPosition(PointPosition commandedPosition, ref PointPosition pointPosition, ref DegradedPointPosition degradedPointPosition)
        {
            pointPosition = commandedPosition;
            switch (_preventedPosition)
            {
                case PreventedPosition.PreventedLeft:
                    if (commandedPosition == PointPosition.Left)
                    {
                        degradedPointPosition = DegradedPointPosition.DegradedLeft;
                        pointPosition = PointPosition.NoEndposition;
                    }
                    break;
                case PreventedPosition.PreventedRight:
                    if (commandedPosition == PointPosition.Right)
                    {
                        degradedPointPosition = DegradedPointPosition.DegradedRight;
                        pointPosition = PointPosition.NoEndposition;
                    }
                    break;
                case PreventedPosition.Trailed:
                    degradedPointPosition = DegradedPointPosition.NotDegraded;
                    pointPosition = PointPosition.UnintendetPosition;
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
            var positions = new Dictionary<PointPosition, string> {
                {PointPosition.Right, "right"},
                {PointPosition.Left, "left"},
                {PointPosition.NoEndposition, "noEndPosition"},
                {PointPosition.UnintendetPosition, "trailed"},
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
