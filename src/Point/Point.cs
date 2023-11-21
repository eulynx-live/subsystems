using System.Net.WebSockets;
using System.Text.Json;
using Google.Protobuf;
using Grpc.Core;
using Sci;
using EulynxLive.Messages.Baseline4R1;
using PointPosition = IPointToInterlockingConnection.PointPosition ;
using DegradedPointPosition = IPointToInterlockingConnection.DegradedPointPosition;
using IPointState = IPointToInterlockingConnection.IPointState;
using static Sci.Rasta;
using System.Text;
using Grpc.Net.Client;
using EulynxLive.Point.Components;
using EulynxLive.Point.Proto;

namespace EulynxLive.Point
{
    public class Point : BackgroundService
    {
        public bool AllPointMachinesCrucial { get; }

        private readonly ILogger<Point> _logger;
        private readonly List<WebSocket> _webSockets;
        private readonly PointToInterlockingConnectionB4R1Impl<Point> _connection;
        private readonly string _localId;
        private readonly string _localRastaId;
        private readonly string _remoteId;
        private readonly string _remoteEndpoint;
        private readonly Random _random;
        private readonly bool _simulateRandomTimeouts;
        private bool _initialized;
        AsyncDuplexStreamingCall<SciPacket, SciPacket>? _currentConnection;
        private readonly IPointState _pointState;
        public IPointState PointState { get { return _pointState; } }

        public Point(ILogger<Point> logger, IConfiguration configuration)
        {
            _webSockets = new List<WebSocket>();
            _connection = new PointToInterlockingConnectionB4R1Impl<Point>(logger, configuration);
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
            _pointState.PointPosition = PointPosition.TRAILED;


            if (_currentConnection != null)
            {
                _connection.SendPointPosition(PointState);
            }

            await UpdateConnectedWebClients();
        }

        private static DegradedPointPosition? GetDegradedPointPosition(PointPosition previousReportedPosition)
            => previousReportedPosition switch
            {
                PointPosition.RIGHT
                    => DegradedPointPosition.DEGRADED_RIGHT,
                PointPosition.LEFT
                    => DegradedPointPosition.DEGRADED_LEFT,
                PointPosition.TRAILED
                    => null,
                PointPosition.NO_ENDPOSITION
                    => null,
                _ => null,
            };

        private static PointPosition? GetPointPositionDegraded(Proto.PointPosition pointPosition) => pointPosition switch
        {
            Proto.PointPosition.NoEndPosition => PointPosition.NO_ENDPOSITION,
            Proto.PointPosition.UnintendedPosition => PointPosition.TRAILED,
            _ => null,
        };

        private void UpdatePointState(PointPosition pointPosition)
        {
            _pointState.PointPosition = pointPosition;
        }

        private void UpdatePointState(PointPosition pointPosition, DegradedPointPosition degradedPointPosition)
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
            _pointState.PointPosition = PointPosition.NO_ENDPOSITION;

            if (_currentConnection != null)
            {
                var degradedPointPosition = AllPointMachinesCrucial ?
                    DegradedPointPosition.NOT_APPLICABLE : GetDegradedPointPosition(_pointState.PointPosition);

                if (degradedPointPosition != null)
                {
                    var pointPosition = GetPointPositionDegraded(message.Position);
                    if (pointPosition != null)
                    {
                        UpdatePointState(pointPosition.Value, degradedPointPosition.Value);
                        _connection.SendPointPosition(PointState);
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

            if (_currentConnection != null)
            {
                if (_pointState.PointPosition != PointPosition.RIGHT &&
                _pointState.PointPosition != PointPosition.LEFT)
                {
                    var reportedDegradedPointPosition = AllPointMachinesCrucial ?
                            DegradedPointPosition.NOT_APPLICABLE : DegradedPointPosition.NOT_DEGRADED;

                    PointPosition? finalPosition = _pointState.DegradedPointPosition switch
                    {
                        DegradedPointPosition.DEGRADED_RIGHT => PointPosition.RIGHT,
                        DegradedPointPosition.DEGRADED_LEFT => PointPosition.LEFT,
                        DegradedPointPosition.NOT_DEGRADED => null,
                        DegradedPointPosition.NOT_APPLICABLE => null,
                        _ => null,
                    };
                    if (finalPosition != null)
                    {
                        UpdatePointState((PointPosition)finalPosition, reportedDegradedPointPosition);
                        _connection.SendPointPosition(PointState);
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
                await Reset();
                try
                {
                    _connection.Setup();

                    var result = await _connection.InitializeConnection(PointState);
                    if (result != 0)
                    {
                        continue;
                    }
                    await UpdateConnectedWebClients();

                    while (true)
                    {
                        var commandedPointPosition = await _connection.ReceivePointPosition();
                        if (commandedPointPosition != null)
                        {
                            break;
                        }

                        if ((commandedPointPosition == PointPosition.LEFT && _pointState.PointPosition == PointPosition.LEFT)
                            || (commandedPointPosition == PointPosition.RIGHT && _pointState.PointPosition == PointPosition.RIGHT))
                        {
                            _connection.SendPointPosition(PointState);
                            continue;
                        }

                        UpdatePointState(PointPosition.NO_ENDPOSITION, DegradedPointPosition.NOT_APPLICABLE);

                        await UpdateConnectedWebClients();

                        // Simulate point movement
                        var transitioningTime = _random.Next(1, 5);
                        var transitioningTask = Task.Delay(transitioningTime * 1000);
                        var pointMovementTimeout = 3 * 1000;

                        _logger.LogDebug("Moving to {}.", commandedPointPosition);

                        if (_simulateRandomTimeouts)
                        {
                            if (await Task.WhenAny(transitioningTask, Task.Delay(pointMovementTimeout)) == transitioningTask)
                            {
                                // transition completed within timeout
                                UpdatePointState(commandedPointPosition.Value);

                                await UpdateConnectedWebClients();

                                _logger.LogInformation("End position reached.");
                                _connection.SendPointPosition(_pointState);
                            }
                            else
                            {
                                // timeout
                                _logger.LogInformation("Timeout");
                                _connection.SendTimeoutMessage();
                            }
                        }
                        else
                        {
                            await transitioningTask;
                            UpdatePointState(commandedPointPosition.Value);

                            await UpdateConnectedWebClients();

                            _logger.LogInformation("End position reached.");
                            _connection.SendPointPosition(_pointState);
                        }
                    }
                }
                catch (RpcException)
                {
                    _logger.LogWarning("Could not communicate with remote endpoint.");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Exception during simulation.");
                }
                finally
                {
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
            var positions = new Dictionary<PointPosition, string> {
                {PointPosition.RIGHT, "right"},
                {PointPosition.LEFT, "left"},
                {PointPosition.NO_ENDPOSITION, "noEndPosition"},
                {PointPosition.TRAILED, "trailed"},
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
