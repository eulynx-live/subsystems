using System.Linq;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sci;
using EulynxLive.Messages.Baseline4R1;
using PointPosition = EulynxLive.Messages.Baseline4R1.PointPointPositionMessageReportedPointPosition;
using DegradedPointPosition = EulynxLive.Messages.Baseline4R1.PointPointPositionMessageReportedDegradedPointPosition;
using static Sci.Rasta;
using System.Text;
using Grpc.Net.Client;
using EulynxLive.Point.Components;
using System;
using EulynxLive.Point.Proto;

namespace EulynxLive.Point
{
    public class Point : BackgroundService
    {
        public bool AllPointMachinesCrucial { get; }

        private readonly ILogger<Point> _logger;
        private readonly List<WebSocket> _webSockets;
        private readonly string _localId;
        private readonly string _localRastaId;
        private readonly string _remoteId;
        private readonly string _remoteEndpoint;
        private readonly Random _random;
        private readonly bool _simulateRandomTimeouts;
        private bool _initialized;
        AsyncDuplexStreamingCall<SciPacket, SciPacket>? _currentConnection;
        private readonly PointMachineState _pointState;
        public PointMachineState PointState { get { return _pointState; } }

        public Point(ILogger<Point> logger, IConfiguration configuration, PointMachineState pointState)
        {
            _logger = logger;
            _webSockets = new List<WebSocket>();
            _currentConnection = null;
            _random = new Random();

            var config = configuration.GetSection("PointSettings").Get<PointConfiguration>() ?? throw new Exception("No configuration provided");
            if (config.AllPointMachinesCrucial == null)
            {
                _logger.LogInformation("Assuming all point machines are crucial.");
            }

            AllPointMachinesCrucial = config.AllPointMachinesCrucial ?? false;
            _localId = config.LocalId;
            _localRastaId = config.LocalRastaId.ToString();
            _remoteId = config.RemoteId;
            _remoteEndpoint = config.RemoteEndpoint;
            _simulateRandomTimeouts = config.SimulateRandomTimeouts ?? false;

            _pointState = pointState;
            _pointState.PointPosition = PointPosition.PointIsInARightHandPositionDefinedEndPosition;
            _pointState.DegradedPointPosition = AllPointMachinesCrucial ? DegradedPointPosition.DegradedPointPositionIsNotApplicable : DegradedPointPosition.PointIsNotInADegradedPosition;
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
            _pointState.PointPosition = PointPosition.PointIsTrailed;

            if (_currentConnection != null)
            {
                var occupancyStatus = new PointPointPositionMessage(_localId, _remoteId, _pointState.PointPosition, _pointState.DegradedPointPosition);
                await _currentConnection.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(occupancyStatus.ToByteArray()) });
            }

            await UpdateConnectedWebClients();
        }

        private static DegradedPointPosition? GetDegradedPointPosition(PointPosition previousReportedPosition)
            => previousReportedPosition switch {
            PointPosition.PointIsInARightHandPositionDefinedEndPosition
                => DegradedPointPosition.PointIsInADegradedRightHandPosition,
            PointPosition.PointIsInALeftHandPositionDefinedEndPosition
                => DegradedPointPosition.PointIsInADegradedLeftHandPosition,
            PointPosition.PointIsInNoEndPosition
                => null,
            PointPosition.PointIsTrailed
                => null,
            _ => null,
        };

        private static PointPosition? GetPointPositionDegraded(Proto.PointPosition pointPosition) => pointPosition switch
        {
            Proto.PointPosition.NoEndPosition => PointPosition.PointIsInNoEndPosition,
            Proto.PointPosition.UnintendedPosition => PointPosition.PointIsTrailed,
            _ => null,
        };

        private void UpdatePointState(PointPosition pointPosition, DegradedPointPosition degradedPointPosition) {
            UpdatePointState(pointPosition);
            _pointState.DegradedPointPosition = degradedPointPosition;
        }

        private void UpdatePointState(PointPosition pointPosition) {
            _pointState.PointPosition = pointPosition;
        }

        /// <summary>
        /// Sets the point into a degraded state.
        /// <c>ReportedPointPosition</c> <param name="message">the PointDegradedMessage.</param>
        /// </summary>
        /// <returns></returns>
        public async Task SetDegraded(PointDegradedMessage message)
        {
            _pointState.PointPosition = PointPosition.PointIsInNoEndPosition;

            if (_currentConnection != null)
            {
                var degradedPointPosition = AllPointMachinesCrucial ?
                    DegradedPointPosition.DegradedPointPositionIsNotApplicable : GetDegradedPointPosition(_pointState.PointPosition);

                if (degradedPointPosition != null)
                {
                    var pointPosition = GetPointPositionDegraded(message.Position);
                    if (pointPosition != null)
                    {
                        UpdatePointState(pointPosition.Value, degradedPointPosition.Value);
                        var occupancyStatus = new PointPointPositionMessage(_localId, _remoteId, _pointState.PointPosition, _pointState.DegradedPointPosition);
                        await _currentConnection.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(occupancyStatus.ToByteArray()) });
                    }
                }
            }

            await UpdateConnectedWebClients();
        }

        /// <summary>
        /// Sets the <c>ReportedPointPosition</c> to the latest final position (left/right).
        /// </summary>
        /// <returns></returns>
        public async Task PutInEndPosition(){

            if (_currentConnection != null)
            {
                if (_pointState.PointPosition != PointPosition.PointIsInARightHandPositionDefinedEndPosition &&
                _pointState.PointPosition != PointPosition.PointIsInALeftHandPositionDefinedEndPosition) {
                    var reportedDegradedPointPosition = AllPointMachinesCrucial ?
                            DegradedPointPosition.DegradedPointPositionIsNotApplicable : DegradedPointPosition.PointIsNotInADegradedPosition;

                    PointPosition? finalPosition = _pointState.DegradedPointPosition switch
                    {
                        DegradedPointPosition.PointIsInADegradedRightHandPosition => PointPosition.PointIsInARightHandPositionDefinedEndPosition,
                        DegradedPointPosition.PointIsInADegradedLeftHandPosition => PointPosition.PointIsInALeftHandPositionDefinedEndPosition,
                        DegradedPointPosition.PointIsNotInADegradedPosition => null,
                        DegradedPointPosition.DegradedPointPositionIsNotApplicable => null,
                        _ => null,
                    };
                    if (finalPosition != null)
                    {
                        UpdatePointState((PointPosition)finalPosition, reportedDegradedPointPosition);
                        var positionMessage = new PointPointPositionMessage(_localId, _remoteId, _pointState.PointPosition, _pointState.DegradedPointPosition);
                        await _currentConnection.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(positionMessage.ToByteArray()) });
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
                    var channel = GrpcChannel.ForAddress(_remoteEndpoint);
                    var client = new RastaClient(channel);
                    _logger.LogTrace("Connecting...");
                    var timeout = new CancellationTokenSource();
                    timeout.CancelAfter(10000);
                    var metadata = new Metadata { { "rasta-id", _localRastaId } };

                    using (_currentConnection = client.Stream(metadata, cancellationToken: timeout.Token))
                    {
                        _logger.LogTrace("Connected. Waiting for request...");
                        if (!await _currentConnection.ResponseStream.MoveNext(timeout.Token)
                            || Message.FromBytes(_currentConnection.ResponseStream.Current.Message.ToByteArray()) is not PointPdiVersionCheckCommand)
                        {
                            _logger.LogError("Unexpected message.");
                            break;
                        }

                        var versionCheckResponse = new PointPdiVersionCheckMessage(_localId, _remoteId, PointPdiVersionCheckMessageResultPdiVersionCheck.PDIVersionsFromReceiverAndSenderDoMatch, /* TODO */ 0, 0, new byte[] { });
                        await _currentConnection.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(versionCheckResponse.ToByteArray()) });

                        if (!await _currentConnection.ResponseStream.MoveNext(timeout.Token)
                            || Message.FromBytes(_currentConnection.ResponseStream.Current.Message.ToByteArray()) is not PointInitialisationRequestCommand)
                        {
                            _logger.LogError("Unexpected message.");
                            break;
                        }

                        var startInitialization = new PointStartInitialisationMessage(_localId, _remoteId);
                        await _currentConnection.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(startInitialization.ToByteArray()) });

                        var initialPosition = new PointPointPositionMessage(_localId, _remoteId, _pointState.PointPosition, _pointState.DegradedPointPosition);
                        await _currentConnection.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(initialPosition.ToByteArray()) });

                        _initialized = true;

                        var completeInitialization = new PointInitialisationCompletedMessage(_localId, _remoteId);
                        await _currentConnection.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(completeInitialization.ToByteArray()) });

                        await UpdateConnectedWebClients();

                        while (true)
                        {
                            if (!await _currentConnection.ResponseStream.MoveNext())
                            {
                                break;
                            }
                            var message = Message.FromBytes(_currentConnection.ResponseStream.Current.Message.ToByteArray());

                            if (message is PointMovePointCommand movePointCommand)
                            {
                                if ((movePointCommand.CommandedPointPosition == PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsARightHandPointMoving && _pointState.PointPosition == PointPosition.PointIsInARightHandPositionDefinedEndPosition)
                                    || (movePointCommand.CommandedPointPosition == PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsALeftHandPointMoving && _pointState.PointPosition == PointPosition.PointIsInALeftHandPositionDefinedEndPosition))
                                {
                                    var response = new PointPointPositionMessage(_localId, _remoteId, _pointState.PointPosition, _pointState.DegradedPointPosition);
                                    await _currentConnection.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(response.ToByteArray()) });
                                    continue;
                                }

                                UpdatePointState(PointPosition.PointIsInNoEndPosition, DegradedPointPosition.DegradedPointPositionIsNotApplicable);

                                await UpdateConnectedWebClients();

                                // Simulate point movement
                                var transitioningTime = _random.Next(1, 5);
                                var transitioningTask = Task.Delay(transitioningTime * 1000);
                                var pointMovementTimeout = 3 * 1000;

                                _logger.LogDebug("Moving to {}.", movePointCommand.CommandedPointPosition);

                                if (_simulateRandomTimeouts)
                                {
                                    if (await Task.WhenAny(transitioningTask, Task.Delay(pointMovementTimeout)) == transitioningTask)
                                    {
                                        // transition completed within timeout
                                        UpdatePointState(
                                            movePointCommand.CommandedPointPosition == PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsARightHandPointMoving
                                                ? PointPosition.PointIsInARightHandPositionDefinedEndPosition
                                                : PointPosition.PointIsInALeftHandPositionDefinedEndPosition
                                        );

                                        await UpdateConnectedWebClients();

                                        _logger.LogInformation("End position reached.");
                                        var response = new PointPointPositionMessage(_localId, _remoteId, _pointState.PointPosition, _pointState.DegradedPointPosition);
                                        await _currentConnection.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(response.ToByteArray()) });
                                    }
                                    else
                                    {
                                        // timeout
                                        _logger.LogInformation("Timeout");
                                        var response = new PointTimeoutMessage(_localId, _remoteId);
                                        await _currentConnection.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(response.ToByteArray()) });
                                    }
                                }
                                else
                                {
                                    await transitioningTask;
                                    UpdatePointState(
                                        movePointCommand.CommandedPointPosition == PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsARightHandPointMoving
                                            ? PointPosition.PointIsInARightHandPositionDefinedEndPosition
                                            : PointPosition.PointIsInALeftHandPositionDefinedEndPosition
                                    );

                                    await UpdateConnectedWebClients();

                                    _logger.LogInformation("End position reached.");
                                    var response = new PointPointPositionMessage(_localId, _remoteId, _pointState.PointPosition, _pointState.DegradedPointPosition);
                                    await _currentConnection.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(response.ToByteArray()) });
                                }
                            }
                            else
                            {
                                _logger.LogInformation("Received unknown message {}", message.GetType());
                            }
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
                {PointPosition.PointIsInARightHandPositionDefinedEndPosition, "right"},
                {PointPosition.PointIsInALeftHandPositionDefinedEndPosition, "left"},
                {PointPosition.PointIsInNoEndPosition, "noEndPosition"},
                {PointPosition.PointIsTrailed, "trailed"},
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
