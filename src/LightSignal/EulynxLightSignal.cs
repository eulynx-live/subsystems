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
using System.Threading.Channels;

namespace EulynxLive.LightSignal
{
    public class LightSignalHostedService : BackgroundService
    {
        private readonly Channel<EulynxMessage> _outgoingMessages;
        private readonly List<EulynxLightSignal> _lightSignals;
        private readonly List<WebSocket> _webSockets;
        private string _remoteEndpoint;
        private string _rastaId;
        private readonly ILogger<LightSignalHostedService> _logger;

        public LightSignalHostedService(LightSignalFactory factory, ILogger<LightSignalHostedService> logger, IConfiguration configuration)
        {
            _outgoingMessages = System.Threading.Channels.Channel.CreateUnbounded<EulynxMessage>();
            _lightSignals = factory.Create(_outgoingMessages);

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

            foreach (var lightSignal in _lightSignals)
            {
                lightSignal.OnUpdate += LightSignal_OnUpdate;
            }
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
            var options = new JsonSerializerOptions { WriteIndented = true };
            var serializedState = JsonSerializer.Serialize(_lightSignals.Select(x => new LightSignalState
            {
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

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
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
                catch (Exception)
                {
                    // _logger.LogWarning(ex, "Exception occurred in signal state machine: Reverting to safe state.");
                    await Task.WhenAll(_lightSignals.Select(x => x.Reset()));
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

    public abstract class EulynxLightSignal
    {
        private readonly ILogger<EulynxLightSignal> _logger;
        private readonly string _remoteId;
        private CancellationTokenSource _ctsSuspend;

        public event EventHandler OnUpdate;

        public BasicAspectType BasicAspectType { get { return BasicAspectType1; } }

        public bool Initialized { get; set; }
        
        public BasicAspectType BasicAspectType1 { get; set; }
        public BasicAspectTypeExtension BasicAspectTypeExtension { get; set; }
        public SpeedIndicators SpeedIndicators { get; set; }
        public SpeedIndicatorsAnnouncements SpeedIndicatorsAnnouncements { get; set; }
        public DirectionIndicators DirectionIndicators { get; set; }
        public DirectionIndicatorsAnnouncements DirectionIndicatorsAnnouncements { get; set; }
        public DowngradeInformation DowngradeInformation { get; set; }
        public RouteInformation RouteInformation { get; set; }
        public IntentionallyDark IntentionallyDark { get; set; }
        public Luminosity Luminosity { get; set; }

        public string Id { get; }

        public CancellationTokenSource CtsSuspend => _ctsSuspend;
        private readonly Channel<EulynxMessage> _incomingMessages;
        private readonly Channel<EulynxMessage> _outgoingMessages;

        protected EulynxLightSignal(ILogger<EulynxLightSignal> logger, string id, string remoteId, Channel<EulynxMessage> outgoingMessages)
        {
            _logger = logger;
            Id = id;
            _remoteId = remoteId;
            _ctsSuspend = new CancellationTokenSource();
            _incomingMessages = System.Threading.Channels.Channel.CreateUnbounded<EulynxMessage>();
            _outgoingMessages = outgoingMessages;
        }

        public async Task Run()
        {
            try
            {
                _logger.LogTrace("Connected. Waiting for request...");
                if (await _incomingMessages.Reader.ReadAsync(CtsSuspend.Token) is not LightSignalVersionCheckCommand)
                {
                    _logger.LogTrace("Unexpected message");
                    return;
                }

                var versionCheckResponse = new LightSignalVersionCheckMessage(Id, _remoteId, PdiVersionCheckResult.Match, /* TODO */ 0, 0);
                await _outgoingMessages.Writer.WriteAsync(versionCheckResponse);

                if (await _incomingMessages.Reader.ReadAsync(CtsSuspend.Token) is not LightSignalInitializationRequestMessage)
                {
                    _logger.LogTrace("Unexpected message");
                    return;
                }

                var startInitialization = new LightSignalStartInitializationMessage(Id, _remoteId);
                await _outgoingMessages.Writer.WriteAsync(startInitialization);

                var indicatedSignalAspect = new LightSignalIndicatedSignalAspectMessage(Id, _remoteId, BasicAspectType1, BasicAspectTypeExtension, SpeedIndicators, SpeedIndicatorsAnnouncements, DirectionIndicators, DirectionIndicatorsAnnouncements, DowngradeInformation, RouteInformation, IntentionallyDark);
                await _outgoingMessages.Writer.WriteAsync(indicatedSignalAspect);

                var setLuminosity = new LightSignalSetLuminosityMessage(Id, _remoteId, Luminosity);
                await _outgoingMessages.Writer.WriteAsync(setLuminosity);

                var completeInitialization = new LightSignalInitializationCompletedMessage(Id, _remoteId);
                await _outgoingMessages.Writer.WriteAsync(completeInitialization);

                Initialized = true;

                TriggerUpdate();

                while (true)
                {
                    _logger.LogTrace("Initialized. Waiting for signal commands...");
                    EulynxMessage message;
                    try
                    {
                        message = await _incomingMessages.Reader.ReadAsync(CtsSuspend.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch
                    {
                        break;
                    }

                    if (message is LightSignalIndicateSignalAspectCommand indicateSignalAspectCommand)
                    {
                        BasicAspectType1 = indicateSignalAspectCommand.BasicAspectType;
                        BasicAspectTypeExtension = indicateSignalAspectCommand.BasicAspectTypeExtension;
                        SpeedIndicators = indicateSignalAspectCommand.SpeedIndicators;
                        SpeedIndicatorsAnnouncements = indicateSignalAspectCommand.SpeedIndicatorsAnnouncements;
                        DirectionIndicators = indicateSignalAspectCommand.DirectionIndicators;
                        DirectionIndicatorsAnnouncements = indicateSignalAspectCommand.DirectionIndicatorsAnnouncements;
                        DowngradeInformation = indicateSignalAspectCommand.DowngradeInformation;
                        RouteInformation = indicateSignalAspectCommand.RouteInformation;
                        IntentionallyDark = indicateSignalAspectCommand.IntentionallyDark;

                        TriggerUpdate();

                        var response = new LightSignalIndicatedSignalAspectMessage(Id, _remoteId, BasicAspectType1, BasicAspectTypeExtension, SpeedIndicators, SpeedIndicatorsAnnouncements, DirectionIndicators, DirectionIndicatorsAnnouncements, DowngradeInformation, RouteInformation, IntentionallyDark);
                        await _outgoingMessages.Writer.WriteAsync(response);
                    }
                }
            } 
            catch (OperationCanceledException)
            {
                var pdiNotAvailable = new LightSignalPDINotAvailableMessage(Id, _remoteId);
                await _outgoingMessages.Writer.WriteAsync(pdiNotAvailable);
                throw;
            }
        }

        public abstract Task Reset();

        protected void TriggerUpdate()
        {
            OnUpdate(this, new EventArgs());
        }

        internal void Suspend()
        {
            CtsSuspend.Cancel();
            TriggerUpdate();
        }

        internal async Task HandleMessage(EulynxMessage eulynxMessage)
        {
            await _incomingMessages.Writer.WriteAsync(eulynxMessage);
        }

        internal async Task Unsuspend()
        {
            if (_ctsSuspend.IsCancellationRequested)
            {
                _ctsSuspend = new CancellationTokenSource();
                var pdiAvailable = new LightSignalPDIAvailableMessage(Id, _remoteId);
                await _outgoingMessages.Writer.WriteAsync(pdiAvailable);
                TriggerUpdate();
            }
        }
    }

    public class MultiSectionLightSignal : EulynxLightSignal
    {
        public MultiSectionLightSignal(ILogger<EulynxLightSignal> logger, string id, string remoteId, Channel<EulynxMessage> outgoingMessages) : base(logger, id, remoteId, outgoingMessages) { }

        public override async Task Reset()
        {
            Initialized = false;
            BasicAspectType1 = BasicAspectType.Stop_Danger_1;
            BasicAspectTypeExtension = BasicAspectTypeExtension.IntendedDark;
            SpeedIndicators = SpeedIndicators.IndicationDark;
            SpeedIndicatorsAnnouncements = SpeedIndicatorsAnnouncements.AnnouncementDark;
            DirectionIndicators = DirectionIndicators.IndicationDark;
            DirectionIndicatorsAnnouncements = DirectionIndicatorsAnnouncements.AnnouncementDark;
            Luminosity = Luminosity.Day;
            TriggerUpdate();
        }
    }

    public class DistantLightSignal : EulynxLightSignal
    {
        public DistantLightSignal(ILogger<EulynxLightSignal> logger, string id, string remoteId, Channel<EulynxMessage> outgoingMessages) : base(logger, id, remoteId, outgoingMessages) { }

        public override async Task Reset()
        {
            Initialized = false;
            BasicAspectType1 = BasicAspectType.ExpectStop;
            BasicAspectTypeExtension = BasicAspectTypeExtension.IntendedDark;
            SpeedIndicators = SpeedIndicators.IndicationDark;
            SpeedIndicatorsAnnouncements = SpeedIndicatorsAnnouncements.AnnouncementDark;
            DirectionIndicators = DirectionIndicators.IndicationDark;
            DirectionIndicatorsAnnouncements = DirectionIndicatorsAnnouncements.AnnouncementDark;
            Luminosity = Luminosity.Day;
            TriggerUpdate();
        }
    }
}
