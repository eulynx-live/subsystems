using System;
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
using EulynxLive.Messages;
using static Sci.Rasta;
using System.Text;
using Grpc.Net.Client;
using TrainDetectionSystem.Components.TDS;

namespace EulynxLive.TrainDetectionSystem
{
    public class TrainDetectionSystem : BackgroundService
    {
        private readonly ILogger<TrainDetectionSystem> _logger;
        private readonly IConfiguration _configuration;
        private readonly List<WebSocket> _webSockets;
        private string _localIdTps;
        private string[] _localIdTvpses;
        private string _localRastaId;
        private string _remoteId;
        private string _remoteEndpoint;
        private byte[] _pps;

        private bool _initialized;
        AsyncDuplexStreamingCall<SciPacket, SciPacket> _currentConnection;
        private Dictionary<string, TvpsOccupancyStatus> _trackSectionStatuses;

        public TDSState TdsState { get; private set; }

        public TrainDetectionSystem(ILogger<TrainDetectionSystem> logger, IConfiguration configuration, TDSState tdsState)
        {
            _logger = logger;
            _configuration = configuration;
            _webSockets = new List<WebSocket>();
            _currentConnection = null;
            _trackSectionStatuses = null;
            TdsState = tdsState;

            // Command line argument parsing.
            var localIdTvps = _configuration["local-id-tvps"];
            if (localIdTvps == null)
            {
                throw new Exception("Missing --local-id-tvps command line parameter.");
            }
            _localIdTvpses = localIdTvps.Split(",");

            _localIdTps = _configuration["local-id-tps"];
            if (_localIdTps == null)
            {
                throw new Exception("Missing --local-id-tps command line parameter.");
            }

            _localRastaId = _configuration["local-rasta-id"];
            if (_localRastaId == null)
            {
                throw new Exception("Missing --local-rasta-id command line parameter.");
            }

            _remoteId = _configuration["remote-id"];
            if (_remoteId == null)
            {
                throw new Exception("Missing --remote-id command line parameter.");
            }

            _remoteEndpoint = _configuration["remote-endpoint"];
            if (_remoteEndpoint == null)
            {
                throw new Exception("Missing --remote-endpoint command line parameter.");
            }

            var pps = _configuration["pps"];
            if (pps == null)
            {
                throw new Exception("Missing --pps command line parameter.");
            }

            _pps = StringToByteArray(pps);
            Array.Reverse(_pps);

            _trackSectionStatuses = _localIdTvpses.Select(x => new { Key = x, Value = TvpsOccupancyStatus.Vacant }).ToDictionary(x => x.Key, x => x.Value);
        }

        private byte[] StringToByteArray(string hex) {
            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
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
                    var cancellationTokenSource = new CancellationTokenSource();
                    cancellationTokenSource.CancelAfter(10000);
                    var metadata = new Metadata { { "rasta-id", _localRastaId } };
                    using (_currentConnection = client.Stream(metadata))
                    {
                        _logger.LogTrace("Connected. Waiting for request...");

                        if (!await _currentConnection.ResponseStream.MoveNext(cancellationTokenSource.Token))
                        {
                            break;
                        }

                        var currentMessage = EulynxMessage.FromBytes(_currentConnection.ResponseStream.Current.Message.ToByteArray());
                        if (!(currentMessage is TrainDetectionSystemVersionCheckCommand))
                        {
                            _logger.LogError($"Unexpected message ({currentMessage.GetType().ToString()}), expected TrainDetectionSystemVersionCheckCommand.");
                            break;
                        }

                        var versionCheckResponse = new Messages.Baseline4R1.TrainDetectionSystemPdiVersionCheckMessage(
                            _localIdTps, _remoteId,
                            Messages.Baseline4R1.TrainDetectionSystemPdiVersionCheckMessageResultPdiVersionCheck.PDIVersionsFromReceiverAndSenderDoMatch,
                            0x1,
                            (byte)_pps.Length,
                            _pps
                        );
                        await _currentConnection.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(versionCheckResponse.ToByteArray()) });

                        if (!await _currentConnection.ResponseStream.MoveNext(cancellationTokenSource.Token))
                        {
                            break;
                        }

                        currentMessage = EulynxMessage.FromBytes(_currentConnection.ResponseStream.Current.Message.ToByteArray());
                        if (!(currentMessage is TrainDetectionSystemInitializationRequestMessage))
                        {
                            _logger.LogError($"Unexpected message ({currentMessage.GetType().ToString()}), expected TrainDetectionSystemInitializationRequestMessage.");
                            break;
                        }

                        var startInitialization = new TrainDetectionSystemStartInitializationMessage(_localIdTps, _remoteId);
                        await _currentConnection.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(startInitialization.ToByteArray()) });

                        foreach (var tps in _localIdTvpses)
                        {
                            var occupancyStatus = new TrainDetectionSystemTvpsOccupancyStatusMessage(tps, _remoteId, _trackSectionStatuses[tps],
                                TvpsAbilityToBeForcedToClear.NotAbleToBeForcedToClear, (ushort)0xffff);
                            await _currentConnection.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(occupancyStatus.ToByteArray()) });
                        }

                        _initialized = true;

                        var completeInitialization = new TrainDetectionSystemInitializationCompletedMessage(_localIdTps, _remoteId);
                        await _currentConnection.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(completeInitialization.ToByteArray()) });

                        await UpdateConnectedWebClients();

                        while (true)
                        {
                            if (!await _currentConnection.ResponseStream.MoveNext(CancellationToken.None))
                            {
                                break;
                            }
                            var message = EulynxMessage.FromBytes(_currentConnection.ResponseStream.Current.Message.ToByteArray());
                            if (message is TrainDetectionSystemForceClearCommand clearCommand)
                            {
                                var tps = clearCommand.ReceiverId.TrimEnd('_');
                                if (clearCommand.ForceClearMode == ForceClearMode.ForceClearU && _trackSectionStatuses.ContainsKey(tps))
                                {
                                    _trackSectionStatuses[tps] = TvpsOccupancyStatus.Vacant;
                                    var occupancyStatus = new TrainDetectionSystemTvpsOccupancyStatusMessage(tps, _remoteId, _trackSectionStatuses[tps],
                                        TvpsAbilityToBeForcedToClear.NotAbleToBeForcedToClear, (ushort)0xffff);
                                    await _currentConnection.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(occupancyStatus.ToByteArray()) });
                                }
                            }

                            await UpdateConnectedWebClients();
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

        public async Task IncreaseAxleCount(string tps)
        {
            if (_trackSectionStatuses == null)
            {
                throw new Exception("TDS is not yet initialized.");
            }

            if (!_trackSectionStatuses.ContainsKey(tps))
            {
                throw new Exception("Unknown track section.");
            }

            if (_trackSectionStatuses[tps] == TvpsOccupancyStatus.Disturbed)
            {
                throw new Exception("Track section status is disturbed.");
            }

            _trackSectionStatuses[tps] = TvpsOccupancyStatus.Occupied;

            if (_currentConnection != null)
            {
                var occupancyStatus = new TrainDetectionSystemTvpsOccupancyStatusMessage(tps, _remoteId, _trackSectionStatuses[tps],
                    TvpsAbilityToBeForcedToClear.NotAbleToBeForcedToClear, (ushort)0xffff);
                await _currentConnection.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(occupancyStatus.ToByteArray()) });
            }

            await UpdateConnectedWebClients();
        }

        public async Task DecreaseAxleCount(string tps)
        {
            if (_trackSectionStatuses == null)
            {
                throw new Exception("TDS is not yet initialized.");
            }

            if (!_trackSectionStatuses.ContainsKey(tps))
            {
                throw new Exception("Unknown track section.");
            }

            if (_trackSectionStatuses[tps] == TvpsOccupancyStatus.Disturbed)
            {
                throw new Exception("Track section status is disturbed.");
            }

            _trackSectionStatuses[tps] = TvpsOccupancyStatus.Vacant;

            if (_currentConnection != null)
            {
                var occupancyStatus = new TrainDetectionSystemTvpsOccupancyStatusMessage(tps, _remoteId, _trackSectionStatuses[tps],
                    TvpsAbilityToBeForcedToClear.NotAbleToBeForcedToClear, (ushort)0xffff);
                await _currentConnection.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(occupancyStatus.ToByteArray()) });
            }

            await UpdateConnectedWebClients();
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
            var options = new JsonSerializerOptions { WriteIndented = true };
            var serializedState = JsonSerializer.Serialize(new
            {
                initialized = _initialized,
                states = _trackSectionStatuses.Select(x => new { Key = x.Key, Value = x.Value.ToString() }).ToDictionary(x => x.Key, x => x.Value)
            }, options);
            var serializedStateBytes = Encoding.UTF8.GetBytes(serializedState);
            await webSocket.SendAsync(serializedStateBytes, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
