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
using ReportedPointPosition = EulynxLive.Messages.Baseline4R1.PointPointPositionMessageReportedPointPosition;
using ReportedDegradedPointPosition = EulynxLive.Messages.Baseline4R1.PointPointPositionMessageReportedDegradedPointPosition;
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
        private readonly ILogger<Point> _logger;
        private readonly IConfiguration _configuration;
        private readonly List<WebSocket> _webSockets;
        private string _localId;
        private string _localRastaId;
        private string _remoteId;
        private string _remoteEndpoint;
        private readonly Random _random;
        private bool _allPointMachinesCrucial;
        private bool _simulateRandomTimeouts;

        private bool _initialized;
        AsyncDuplexStreamingCall<SciPacket, SciPacket> _currentConnection;
        private PointMachineState _pointState;
        public PointMachineState PointState { get { return _pointState; } }

        public record PointConfiguration(
            string LocalId,
            int LocalRastaId,
            string RemoteId,
            string RemoteEndpoint,
            bool? allPointMachinesCrucial = null,
            bool? SimulateRandomTimeouts = null
        );


        public Point(ILogger<Point> logger, IConfiguration configuration, PointMachineState pointState)
        {
            _logger = logger;
            _configuration = configuration;
            _webSockets = new List<WebSocket>();
            _currentConnection = null;
            _random = new Random();

            _pointState = pointState;
            _pointState.PointPosition = ReportedPointPosition.PointIsInARightHandPositionDefinedEndPosition;
            _pointState.DegradedPointPosition = _allPointMachinesCrucial? ReportedDegradedPointPosition.DegradedPointPositionIsNotApplicable : ReportedDegradedPointPosition.PointIsNotInADegradedPosition;
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
                    ArraySegment<byte> buffer = new ArraySegment<byte>(messageBuffer);
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

        public async Task SimulateTrailed()
        {
            _pointState.PointPosition = ReportedPointPosition.PointIsTrailed;

            if (_currentConnection != null)
            {
                var occupancyStatus = new PointPointPositionMessage(_localId, _remoteId, _pointState.PointPosition, _pointState.DegradedPointPosition);
                await _currentConnection.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(occupancyStatus.ToByteArray()) });
            }

            await UpdateConnectedWebClients();
        }

        private ReportedDegradedPointPosition? GetReportedDegradedPointPosition(ReportedPointPosition previousReportedPosition) => previousReportedPosition switch
        {
            ReportedPointPosition.PointIsInARightHandPositionDefinedEndPosition
                => ReportedDegradedPointPosition.PointIsInADegradedRightHandPosition,
            ReportedPointPosition.PointIsInALeftHandPositionDefinedEndPosition
                => ReportedDegradedPointPosition.PointIsInADegradedLeftHandPosition,
            ReportedPointPosition.PointIsInNoEndPosition
                => null,
            ReportedPointPosition.PointIsTrailed
                => null,
            _ => null,
        };

        private ReportedPointPosition? GetReportedPointPositionDegraded(PointPosition pointPosition) => pointPosition switch
        {
            PointPosition.NoEndPosition => ReportedPointPosition.PointIsInNoEndPosition,
            PointPosition.Trailed => ReportedPointPosition.PointIsTrailed,
            _ => null,
        };

        private void UpdatePointState(ReportedPointPosition pointPosition, ReportedDegradedPointPosition degradedPointPosition) {
            UpdatePointState(pointPosition);
            _pointState.DegradedPointPosition = degradedPointPosition;
        }
        private void UpdatePointState(ReportedPointPosition pointPosition) {
            _pointState.PointPosition = pointPosition;
        }


        /// <summary>
        /// Sets the point into a degraded state. 
        /// <c>ReportedPointPosition</c> <param name="message">the PointDegradedMessage.</param>
        /// </summary>
        /// <returns></returns>
        public async Task SetDegraded(PointDegradedMessage message)
        {
            _pointState.PointPosition = ReportedPointPosition.PointIsInNoEndPosition;

            if (_currentConnection != null)
            {
                ReportedDegradedPointPosition? reportedDegradedPointPosition = _allPointMachinesCrucial ?
                    ReportedDegradedPointPosition.DegradedPointPositionIsNotApplicable :
                    GetReportedDegradedPointPosition(_pointState.PointPosition);

                if (reportedDegradedPointPosition != null)
                {
                    var reportedPointPosition = GetReportedPointPositionDegraded(message.Position);
                    if (reportedPointPosition != null)
                    {
                        UpdatePointState((ReportedPointPosition)reportedPointPosition, (ReportedDegradedPointPosition)reportedDegradedPointPosition);
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
        public async Task FinalizePosition(){
            
            if (_currentConnection != null)
            {
                if (_pointState.PointPosition != ReportedPointPosition.PointIsInARightHandPositionDefinedEndPosition &&
                _pointState.PointPosition != ReportedPointPosition.PointIsInALeftHandPositionDefinedEndPosition) {
                    var reportedDegradedPointPosition = _allPointMachinesCrucial ?
                            ReportedDegradedPointPosition.DegradedPointPositionIsNotApplicable : ReportedDegradedPointPosition.PointIsNotInADegradedPosition;

                    ReportedPointPosition? finalPosition = _pointState.DegradedPointPosition switch
                    {
                        ReportedDegradedPointPosition.PointIsInADegradedRightHandPosition => ReportedPointPosition.PointIsInARightHandPositionDefinedEndPosition,
                        ReportedDegradedPointPosition.PointIsInADegradedLeftHandPosition => ReportedPointPosition.PointIsInALeftHandPositionDefinedEndPosition,
                        ReportedDegradedPointPosition.PointIsNotInADegradedPosition => null,
                        ReportedDegradedPointPosition.DegradedPointPositionIsNotApplicable => null,
                        _ => null,
                    };
                    if (finalPosition != null) 
                    {
                        UpdatePointState((ReportedPointPosition)finalPosition, reportedDegradedPointPosition);
                        var positionMessage = new PointPointPositionMessage(_localId, _remoteId, _pointState.PointPosition, _pointState.DegradedPointPosition);
                        await _currentConnection.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(positionMessage.ToByteArray()) });
                    } 
                }
            }
            await UpdateConnectedWebClients();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var config = _configuration.GetSection("PointSettings").Get<PointConfiguration>();
                if (config.allPointMachinesCrucial == null)
                {
                    _logger.LogInformation("Assuming all point machines are critical");
                }

                _allPointMachinesCrucial = config.allPointMachinesCrucial ?? false;
                _localId = config.LocalId;
                _localRastaId = config.LocalRastaId.ToString();
                _remoteId = config.RemoteId;
                _remoteEndpoint = config.RemoteEndpoint;
                _simulateRandomTimeouts = config.SimulateRandomTimeouts ?? false;
            }
            catch (Exception e)
            {
                _logger.LogError($"Usage: --PointSettings:LocalId=<> --PointSettings:LocalRastaId=<> --PointSettings:RemoteId=<> --PointSettings:RemoteEndpoint=<>. \n {e.Message}");

                throw;
            }


            // Main loop.

            while (true)
            {
                await Reset();
                try
                {
                    var channel = GrpcChannel.ForAddress(_remoteEndpoint);
                    var client = new RastaClient(channel);
                    _logger.LogTrace("Connecting...");
                    var cancellationTokenSource = new CancellationTokenSource();
                    cancellationTokenSource.CancelAfter(10000);
                    var metadata = new Metadata { { "rasta-id", _localRastaId } };

                    using (_currentConnection = client.Stream(metadata))
                    {
                        _logger.LogTrace("Connected. Waiting for request...");
                        if (!await _currentConnection.ResponseStream.MoveNext(cancellationTokenSource.Token)
                            || !(Message.FromBytes(_currentConnection.ResponseStream.Current.Message.ToByteArray()) is PointPdiVersionCheckCommand))
                        {
                            _logger.LogError("Unexpected message.");
                            break;
                        }

                        var versionCheckResponse = new PointPdiVersionCheckMessage(_localId, _remoteId, PointPdiVersionCheckMessageResultPdiVersionCheck.PDIVersionsFromReceiverAndSenderDoMatch, /* TODO */ 0, 0, new byte[] { });
                        await _currentConnection.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(versionCheckResponse.ToByteArray()) });

                        if (!await _currentConnection.ResponseStream.MoveNext(cancellationTokenSource.Token)
                            || !(Message.FromBytes(_currentConnection.ResponseStream.Current.Message.ToByteArray()) is PointInitialisationRequestCommand))
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
                                if ((movePointCommand.CommandedPointPosition == PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsARightHandPointMoving && _pointState.PointPosition == ReportedPointPosition.PointIsInARightHandPositionDefinedEndPosition)
                                    || (movePointCommand.CommandedPointPosition == PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsALeftHandPointMoving && _pointState.PointPosition == ReportedPointPosition.PointIsInALeftHandPositionDefinedEndPosition))
                                {
                                    var response = new PointPointPositionMessage(_localId, _remoteId, _pointState.PointPosition, _pointState.DegradedPointPosition);
                                    await _currentConnection.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(response.ToByteArray()) });
                                    continue;
                                }

                                UpdatePointState(ReportedPointPosition.PointIsInNoEndPosition, ReportedDegradedPointPosition.DegradedPointPositionIsNotApplicable);
                                
                                await UpdateConnectedWebClients();

                                // Simulate point movement
                                var transitioningTime = _random.Next(1, 5);
                                var transitioningTask = Task.Delay(transitioningTime * 1000);
                                var timeout = 3;

                                _logger.LogDebug("Moving to {}.", movePointCommand.CommandedPointPosition);

                                if (_simulateRandomTimeouts != null)
                                {
                                    if (await Task.WhenAny(transitioningTask, Task.Delay(timeout)) == transitioningTask)
                                    {
                                        // transition completed within timeout
                                        UpdatePointState(
                                            movePointCommand.CommandedPointPosition == PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsARightHandPointMoving
                                                ? ReportedPointPosition.PointIsInARightHandPositionDefinedEndPosition
                                                : ReportedPointPosition.PointIsInALeftHandPositionDefinedEndPosition
                                        );

                                        await UpdateConnectedWebClients();

                                        _logger.LogDebug("End position reached.");
                                        var response = new PointPointPositionMessage(_localId, _remoteId, _pointState.PointPosition, _pointState.DegradedPointPosition);
                                        await _currentConnection.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(response.ToByteArray()) });
                                    }
                                    else
                                    {
                                        // timeout
                                        _logger.LogDebug("Timeout");
                                        var response = new PointTimeoutMessage(_localId, _remoteId);
                                        await _currentConnection.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(response.ToByteArray()) });
                                    }
                                }
                                else
                                {
                                    await transitioningTask;
                                    UpdatePointState(
                                        movePointCommand.CommandedPointPosition == PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsARightHandPointMoving
                                            ? ReportedPointPosition.PointIsInARightHandPositionDefinedEndPosition
                                            : ReportedPointPosition.PointIsInALeftHandPositionDefinedEndPosition
                                    );

                                    await UpdateConnectedWebClients();

                                    _logger.LogDebug("End position reached.");
                                    var response = new PointPointPositionMessage(_localId, _remoteId, _pointState.PointPosition, _pointState.DegradedPointPosition);
                                    await _currentConnection.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(response.ToByteArray()) });
                                }
                            }
                            else
                            {
                                _logger.LogInformation($"Received unknown message {message.GetType().ToString()}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Exception during simulation. Resetting.");
                    await Reset();
                    await Task.Delay(1000);
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
            var positions = new Dictionary<ReportedPointPosition, string> {
                {ReportedPointPosition.PointIsInARightHandPositionDefinedEndPosition, "right"},
                {ReportedPointPosition.PointIsInALeftHandPositionDefinedEndPosition, "left"},
                {ReportedPointPosition.PointIsInNoEndPosition, "noEndPosition"},
                {ReportedPointPosition.PointIsTrailed, "trailed"},
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
