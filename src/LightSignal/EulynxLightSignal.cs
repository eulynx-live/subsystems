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
using Microsoft.Extensions.Configuration;
using Grpc.Net.Client;

namespace EulynxLive.LightSignal
{
    public abstract class EulynxLightSignal : BackgroundService
    {
        private readonly ILogger<EulynxLightSignal> _logger;
        protected readonly IConfiguration _configuration;
        private readonly List<WebSocket> _webSockets;

        protected bool _initialized;
        protected BasicAspectType _basicAspectType;
        protected BasicAspectTypeExtension _basicAspectTypeExtension;
        protected SpeedIndicators _speedIndicators;
        protected SpeedIndicatorsAnnouncements _speedIndicatorsAnnouncements;
        protected DirectionIndicators _directionIndicators;
        protected DirectionIndicatorsAnnouncements _directionIndicatorsAnnouncements;
        private DowngradeInformation _downgradeInformation;
        private RouteInformation _routeInformation;
        private IntentionallyDark _intentionallyDark;
        protected Luminosity _luminosity;

        public BasicAspectType BasicAspectType { get { return _basicAspectType; }}

        public EulynxLightSignal(ILogger<EulynxLightSignal> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _webSockets = new List<WebSocket>();
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
            var localId = _configuration["local-id"];
            if (localId == null)
            {
                throw new Exception("Missing --local-id command line parameter.");
            }

            var localRastaId = _configuration["local-rasta-id"];
            if (localRastaId == null)
            {
                throw new Exception("Missing --local-rasta-id command line parameter.");
            }

            var remoteId = _configuration["remote-id"];
            if (remoteId == null)
            {
                throw new Exception("Missing --remote-id command line parameter.");
            }

            var remoteEndpoint = _configuration["remote-endpoint"];
            if (remoteEndpoint == null)
            {
                throw new Exception("Missing --remote-endpoint command line parameter.");
            }

            while (true)
            {
                await Reset();
                try
                {
                    var channel = GrpcChannel.ForAddress(remoteEndpoint);
                    var client = new RastaClient(channel);
                    _logger.LogTrace("Connecting...");
                    var metadata = new Metadata { { "rasta-id", localRastaId } };
                    using (var stream = client.Stream(metadata))
                    {
                        _logger.LogTrace("Connected. Waiting for request...");
                        if (!await stream.ResponseStream.MoveNext()
                            || !(EulynxMessage.FromBytes(stream.ResponseStream.Current.Message.ToByteArray()) is LightSignalVersionCheckCommand))
                        {
                            _logger.LogTrace("Unexpected message");
                            break;
                        }

                        var versionCheckResponse = new LightSignalVersionCheckMessage(localId, remoteId, PdiVersionCheckResult.Match, /* TODO */ 0, 0);
                        await stream.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(versionCheckResponse.ToByteArray()) });

                        if (!await stream.ResponseStream.MoveNext()
                            || !(EulynxMessage.FromBytes(stream.ResponseStream.Current.Message.ToByteArray()) is LightSignalInitializationRequestMessage))
                        {
                            _logger.LogTrace("Unexpected message");
                            break;
                        }

                        var startInitialization = new LightSignalStartInitializationMessage(localId, remoteId);
                        await stream.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(startInitialization.ToByteArray()) });

                        var indicatedSignalAspect = new LightSignalIndicatedSignalAspectMessage(localId, remoteId, _basicAspectType, _basicAspectTypeExtension, _speedIndicators, _speedIndicatorsAnnouncements, _directionIndicators, _directionIndicatorsAnnouncements, _downgradeInformation, _routeInformation, _intentionallyDark);
                        await stream.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(indicatedSignalAspect.ToByteArray()) });

                        var setLuminosity = new LightSignalSetLuminosityMessage(localId, remoteId, _luminosity);
                        await stream.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(setLuminosity.ToByteArray()) });

                        var completeInitialization = new LightSignalInitializationCompletedMessage(localId, remoteId);
                        await stream.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(completeInitialization.ToByteArray()) });

                        _initialized = true;

                        while (true)
                        {
                            _logger.LogTrace("Initialized. Waiting for signal commands...");
                            if (!await stream.ResponseStream.MoveNext())
                            {
                                break;
                            }
                            var message = EulynxMessage.FromBytes(stream.ResponseStream.Current.Message.ToByteArray());

                            if (message is LightSignalIndicateSignalAspectCommand indicateSignalAspectCommand)
                            {
                                _basicAspectType = indicateSignalAspectCommand.BasicAspectType;
                                _basicAspectTypeExtension = indicateSignalAspectCommand.BasicAspectTypeExtension;
                                _speedIndicators = indicateSignalAspectCommand.SpeedIndicators;
                                _speedIndicatorsAnnouncements = indicateSignalAspectCommand.SpeedIndicatorsAnnouncements;
                                _directionIndicators = indicateSignalAspectCommand.DirectionIndicators;
                                _directionIndicatorsAnnouncements = indicateSignalAspectCommand.DirectionIndicatorsAnnouncements;
                                _downgradeInformation = indicateSignalAspectCommand.DowngradeInformation;
                                _routeInformation = indicateSignalAspectCommand.RouteInformation;
                                _intentionallyDark = indicateSignalAspectCommand.IntentionallyDark;

                                await UpdateConnectedWebClients();

                                var response = new LightSignalIndicatedSignalAspectMessage(localId, remoteId, _basicAspectType, _basicAspectTypeExtension, _speedIndicators, _speedIndicatorsAnnouncements, _directionIndicators, _directionIndicatorsAnnouncements, _downgradeInformation, _routeInformation, _intentionallyDark);
                                await stream.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(response.ToByteArray()) });
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    // _logger.LogWarning(ex, "Exception occurred in signal state machine: Reverting to safe state.");
                    await Reset();
                    await Task.Delay(1000);
                }
            }
        }

        protected abstract Task Reset();

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
            var serializedState = JsonSerializer.Serialize(new LightSignalState
            {
                setup = _initialized,
                mainAspect = (MainAspect)_basicAspectType,
                secondaryAspect = (SecondaryAspect)_basicAspectTypeExtension,
                zs3 = (Zs3v)_speedIndicators,
                zs3v = (Zs3v)_speedIndicatorsAnnouncements,
                zs2 = (Zs2v)_directionIndicators,
                zs2v = (Zs2v)_directionIndicatorsAnnouncements,
                luminosity = (LuminosityNeuPro)_intentionallyDark
            }, options);
            var serializedStateBytes = Encoding.UTF8.GetBytes(serializedState);
            await webSocket.SendAsync(serializedStateBytes, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }

    public class MultiSectionLightSignal : EulynxLightSignal
    {
        public MultiSectionLightSignal(ILogger<EulynxLightSignal> logger, IConfiguration configuration):base(logger,configuration){}
        protected override async Task Reset()
        {
            _initialized = false;
            _basicAspectType = BasicAspectType.Stop_Danger_1;
            _basicAspectTypeExtension = BasicAspectTypeExtension.IntendedDark;
            _speedIndicators = SpeedIndicators.IndicationDark;
            _speedIndicatorsAnnouncements = SpeedIndicatorsAnnouncements.AnnouncementDark;
            _directionIndicators = DirectionIndicators.IndicationDark;
            _directionIndicatorsAnnouncements = DirectionIndicatorsAnnouncements.AnnouncementDark;
            _luminosity = Luminosity.Day;
            await UpdateConnectedWebClients();
        }
    }

    public class DistantLightSignal : EulynxLightSignal
    {
        public DistantLightSignal(ILogger<EulynxLightSignal> logger, IConfiguration configuration):base(logger,configuration){}
        protected override async Task Reset()
        {
            _initialized = false;
            _basicAspectType = BasicAspectType.ExpectStop;
            _basicAspectTypeExtension = BasicAspectTypeExtension.IntendedDark;
            _speedIndicators = SpeedIndicators.IndicationDark;
            _speedIndicatorsAnnouncements = SpeedIndicatorsAnnouncements.AnnouncementDark;
            _directionIndicators = DirectionIndicators.IndicationDark;
            _directionIndicatorsAnnouncements = DirectionIndicatorsAnnouncements.AnnouncementDark;
            _luminosity = Luminosity.Day;
            await UpdateConnectedWebClients();
        }
    }
}
