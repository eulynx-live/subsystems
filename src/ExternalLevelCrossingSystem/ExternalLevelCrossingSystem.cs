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

namespace EulynxLive.ExternalLevelCrossingSystem
{
    public class ExternalLevelCrossingSystem : BackgroundService
    {
        private readonly ILogger<ExternalLevelCrossingSystem> _logger;
        private readonly IConfiguration _configuration;
        private readonly List<WebSocket> _webSockets;
        private string _localId;
        private string _localRastaId;
        private string _remoteId;
        private string _remoteEndpoint;
        private readonly Random _random;

        private bool _initialized;
        AsyncDuplexStreamingCall<SciPacket, SciPacket> _currentConnection;

        public ExternalLevelCrossingSystem(ILogger<ExternalLevelCrossingSystem> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _webSockets = new List<WebSocket>();
            _currentConnection = null;
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
            // Command line argument parsing.
            _localId = _configuration["local-id"];
            if (_localId == null) {
                throw new Exception("Missing --local-id command line parameter.");
            }

            _localRastaId = _configuration["local-rasta-id"];
            if (_localRastaId == null) {
                throw new Exception("Missing --local-rasta-id command line parameter.");
            }

            _remoteId = _configuration["remote-id"];
            if (_remoteId == null) {
                throw new Exception("Missing --remote-id command line parameter.");
            }

            _remoteEndpoint = _configuration["remote-endpoint"];
            if (_remoteEndpoint == null) {
                throw new Exception("Missing --remote-endpoint command line parameter.");
            }

            var simulateRandomTimeouts = _configuration["simulate-timeouts"];

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
                    var metadata = new Metadata {{"rasta-id", _localRastaId}};
                    using (_currentConnection = client.Stream(metadata))
                    {
                        _logger.LogTrace("Connected. Waiting for request...");
                        if (!await _currentConnection.ResponseStream.MoveNext(cancellationTokenSource.Token)
                            || !(EulynxMessage.FromBytes(_currentConnection.ResponseStream.Current.Message.ToByteArray()) is ExternalLevelCrossingSystemVersionCheckCommand))
                        {
                            _logger.LogError("Unexpected message.");
                            break;
                        }

                        var versionCheckResponse = new ExternalLevelCrossingSystemVersionCheckMessage(_localId, _remoteId, PdiVersionCheckResult.Match, /* TODO */ 0, 0);
                        await _currentConnection.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(versionCheckResponse.ToByteArray()) });

                        if (!await _currentConnection.ResponseStream.MoveNext(cancellationTokenSource.Token)
                            || !(EulynxMessage.FromBytes(_currentConnection.ResponseStream.Current.Message.ToByteArray()) is ExternalLevelCrossingSystemInitializationRequestMessage))
                        {
                            _logger.LogError("Unexpected message.");
                            break;
                        }

                        var startInitialization = new ExternalLevelCrossingSystemStartInitializationMessage(_localId, _remoteId);
                        await _currentConnection.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(startInitialization.ToByteArray()) });

                        // var initialPosition = new ExternalLevelCrossingSystemPositionMessage(_localId, _remoteId, _position);
                        // await _currentConnection.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(initialPosition.ToByteArray()) });

                        _initialized = true;

                        var completeInitialization = new ExternalLevelCrossingSystemInitializationCompletedMessage(_localId, _remoteId);
                        await _currentConnection.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(completeInitialization.ToByteArray()) });

                        await UpdateConnectedWebClients();

                        while (true)
                        {
                            if (!await _currentConnection.ResponseStream.MoveNext())
                            {
                                break;
                            }
                            var message = EulynxMessage.FromBytes(_currentConnection.ResponseStream.Current.Message.ToByteArray());

                            _logger.LogInformation($"Received unknown message {message.GetType().ToString()}");
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
            var options = new JsonSerializerOptions { WriteIndented = true };
            var serializedState = JsonSerializer.Serialize(new {
                initialized = _initialized,
            }, options);
            var serializedStateBytes = Encoding.UTF8.GetBytes(serializedState);
            await webSocket.SendAsync(serializedStateBytes, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
