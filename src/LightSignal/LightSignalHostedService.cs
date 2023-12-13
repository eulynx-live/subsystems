using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sci;
using EulynxLive.Messages;
using static Sci.Rasta;
using System.Text;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using EulynxLive.Messages.Deprecated;

namespace EulynxLive.LightSignal
{
    public class LightSignalHostedService : BackgroundService
    {
        private readonly List<WebSocket> _webSockets;
        private string _remoteEndpoint;
        private readonly LightSignalFactory _factory;
        private string _rastaId;
        private List<EulynxLightSignal> _lightSignals;
        private readonly ILogger<LightSignalHostedService> _logger;

        public LightSignalHostedService(LightSignalFactory factory, ILogger<LightSignalHostedService> logger, IConfiguration configuration)
        {
            _factory = factory;

            _rastaId = configuration["local-rasta-id"];
            if (_rastaId == null)
            {
                throw new Exception("Missing --local-rasta-id command line parameter.");
            }

            _remoteEndpoint = configuration["remote-endpoint"];
            if (_remoteEndpoint == null)
            {
                throw new Exception("Missing --remote-endpoint command line parameter.");
            }

            _webSockets = new List<WebSocket>();
            _logger = logger;
        }

        private async void LightSignal_OnUpdate(object sender, EventArgs e)
        {
            await UpdateConnectedWebClients();
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

        protected async Task UpdateConnectedWebClients()
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
            if (_lightSignals != null) {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var serializedState = JsonSerializer.Serialize(_lightSignals.Select(x => new LightSignalState{
                    id = x.Id,
                    suspended = x.CtsSuspend.IsCancellationRequested,
                    setup = x.Initialized,
                    mainAspect = (MainAspect)x.BasicAspectType,
                    secondaryAspect = (SecondaryAspect)x.BasicAspectTypeExtension,
                    zs3 = (Zs3v)x.SpeedIndicators,
                    zs3v = (Zs3v)x.SpeedIndicatorsAnnouncements,
                    zs2 = (Zs2v)x.DirectionIndicators,
                    zs2v = (Zs2v)x.DirectionIndicatorsAnnouncements,
                    luminosity = (LuminosityNeuPro)x.IntentionallyDark
                }), options);
                var serializedStateBytes = Encoding.UTF8.GetBytes(serializedState);
                await webSocket.SendAsync(serializedStateBytes, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                var _outgoingMessages = System.Threading.Channels.Channel.CreateUnbounded<EulynxMessage>();
                _lightSignals = _factory.Create(_outgoingMessages);

                foreach (var lightSignal in _lightSignals)
                {
                    lightSignal.OnUpdate += LightSignal_OnUpdate;
                }

                await Task.WhenAll(_lightSignals.Select(x => x.Reset()));

                try
                {
                    var channel = GrpcChannel.ForAddress(_remoteEndpoint);
                    var client = new RastaClient(channel);
                    _logger.LogTrace("Connecting...");
                    var metadata = new Metadata { { "rasta-id", _rastaId } };

                    using var stream = client.Stream(metadata);

                    // Mux
                    var mux = async () => {
                        await foreach (var message in _outgoingMessages.Reader.ReadAllAsync())
                        {
                            var packet = new SciPacket
                            {
                                Message = ByteString.CopyFrom(message.ToByteArray()),
                            };
                            _logger.LogTrace("Writing bytes to connection: {bytes}", packet.Message.ToByteArray());
                            await stream.RequestStream.WriteAsync(packet);
                        }
                    };

                    // Demux
                    var demux = async () => {
                        await foreach (var message in stream.ResponseStream.ReadAllAsync())
                        {
                            var bytes = message.Message.ToByteArray();
                            _logger.LogTrace("Received bytes from connection: {bytes}", bytes);
                            EulynxMessage eulynxMessage;
                            try
                            {
                                eulynxMessage = EulynxMessage.FromBytes(bytes);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(ex, "Couldn't parse EULYNX message");
                                throw;
                            }

                            var ls = _lightSignals.SingleOrDefault(x => x.Id == eulynxMessage.ReceiverId.TrimEnd('_'));
                            if (ls != null)
                            {
                                await ls.HandleMessage(eulynxMessage);
                            }
                        }
                    };

                    await Task.WhenAny(_lightSignals.Select(async x => {
                        while (true)
                        {
                            try
                            {
                                await x.Run();
                            }
                            catch (Exception)
                            {
                                await x.Reset();
                                await Task.Delay(1000);
                            }
                        }
                    }).Concat(new[] { mux(), demux() }));
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Exception occurred in signal state machine: Reverting to safe state.");
                }

                foreach (var lightSignal in _lightSignals)
                {
                    lightSignal.CtsSuspend.Cancel();
                }

                await Task.WhenAll(_lightSignals.Select(x => x.Reset()));

                foreach (var lightSignal in _lightSignals)
                {
                    lightSignal.OnUpdate -= LightSignal_OnUpdate;
                }
            }
        }

        internal void Suspend(string lightSignal)
        {
            var ls = _lightSignals.SingleOrDefault(x => x.Id == lightSignal);
            if (ls != null)
            {
                ls.Suspend();
            }
        }

        internal async Task Unsuspend(string lightSignal)
        {
            var ls = _lightSignals.SingleOrDefault(x => x.Id == lightSignal);
            if (ls != null)
            {
                await ls.Unsuspend();
            }
        }
    }
}
