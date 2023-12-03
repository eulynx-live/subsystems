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

        private readonly ILogger<Point> _logger;
        private readonly Func<Task> _simulateTimeout;
        private readonly List<WebSocket> _webSockets;
        private readonly IPointToInterlockingConnection _connection;
        private readonly Random _random;
        private readonly bool _simulateRandomTimeouts;
        private bool _initialized;
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
                PointPosition = GenericPointPosition.NoEndPosition,
                DegradedPointPosition = AllPointMachinesCrucial ? GenericDegradedPointPosition.NotApplicable : GenericDegradedPointPosition.NotDegraded
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

        public async Task SimulateUnintendedPosition()
        {
            _pointState.PointPosition = GenericPointPosition.UnintendedPosition;


            if (_connection != null)
            {
                await _connection.SendPointPosition(PointState);
            }

            await UpdateConnectedWebClients();
        }

        private static GenericDegradedPointPosition? GetDegradedPointPosition(GenericPointPosition previousReportedPosition)
            => previousReportedPosition switch
            {
                GenericPointPosition.Right
                    => GenericDegradedPointPosition.DegradedRight,
                GenericPointPosition.Left
                    => GenericDegradedPointPosition.DegradedLeft,
                GenericPointPosition.UnintendedPosition
                    => null,
                GenericPointPosition.NoEndPosition
                    => null,
                _ => null,
            };

        private static GenericPointPosition? GetPointPositionDegraded(PointPosition pointPosition) => pointPosition switch
        {
            PointPosition.NoEndPosition => GenericPointPosition.NoEndPosition,
            PointPosition.UnintendedPosition => GenericPointPosition.UnintendedPosition,
            _ => null,
        };

        private void UpdatePointState(GenericPointPosition pointPosition)
        {
            _pointState.PointPosition = pointPosition;
        }

        private void UpdatePointState(GenericPointPosition pointPosition, GenericDegradedPointPosition degradedPointPosition)
        {
            _pointState.PointPosition = pointPosition;
            _pointState.DegradedPointPosition = degradedPointPosition;
        }

        /// <summary>
        /// Sets the point into a degraded state.
        /// <c>ReportedPointPosition</c> <param name="message">the PointDegradedMessage.</param>
        /// </summary>
        /// <returns></returns>
        public async Task SetDegraded(PointDegradedMessage message)
        {
            _pointState.PointPosition = GenericPointPosition.NoEndPosition;

            if (_connection != null)
            {
                var degradedPointPosition = AllPointMachinesCrucial ?
                    GenericDegradedPointPosition.NotApplicable : GetDegradedPointPosition(_pointState.PointPosition);

                if (degradedPointPosition != null)
                {
                    var pointPosition = GetPointPositionDegraded(message.Position);
                    if (pointPosition != null)
                    {
                        UpdatePointState(pointPosition.Value, degradedPointPosition.Value);
                        await _connection.SendPointPosition(PointState);
                    }
                }
            }

            await UpdateConnectedWebClients();
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
                    var reportedDegradedPointPosition = AllPointMachinesCrucial ?
                            GenericDegradedPointPosition.NotApplicable : GenericDegradedPointPosition.NotDegraded;

                    PointPosition? finalPosition = _pointState.DegradedPointPosition switch
                    {
                        GenericDegradedPointPosition.DegradedRight => PointPosition.Right,
                        GenericDegradedPointPosition.DegradedLeft => PointPosition.Left,
                        GenericDegradedPointPosition.NotDegraded => null,
                        GenericDegradedPointPosition.NotApplicable => null,
                        _ => null,
                    };
                    if (finalPosition != null)
                    {
                        UpdatePointState((GenericPointPosition)finalPosition, reportedDegradedPointPosition);
                        await _connection.SendPointPosition(PointState);
                    }
                }
            }
            await UpdateConnectedWebClients();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Main loop.
            while (true)
            {
                _logger.LogTrace("Connecting...");
                var metadata = new Metadata { { "rasta-id", _connection.Configuration.LocalRastaId.ToString() } };
                var grpc = new GrpcConnection(metadata, _connection.Configuration.RemoteEndpoint, _connection.TimeoutToken);
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
                            await _connection.SendPointPosition(PointState);
                            continue;
                        }

                        UpdatePointState(GenericPointPosition.NoEndPosition, GenericDegradedPointPosition.NotApplicable);

                        await UpdateConnectedWebClients();

                        // Simulate point movement
                        var transitioningTime = _random.Next(1, 5);
                        var transitioningTask = Task.Delay(transitioningTime * 1000, CancellationToken.None);
                        var pointMovementTimeout = 3 * 1000;

                        _logger.LogDebug("Moving to {}.", commandedPointPosition);

                        if (_simulateRandomTimeouts)
                        {
                            if (await Task.WhenAny(transitioningTask, Task.Delay(pointMovementTimeout, CancellationToken.None)) == transitioningTask)
                            {
                                // transition completed within timeout
                                UpdatePointState(commandedPointPosition.Value);

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
