using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using EulynxLive.Messages;
using System.Threading.Channels;
using EulynxLive.Messages.Deprecated;

namespace EulynxLive.LightSignal
{

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
                    } else {
                        throw new Exception($"Unexpected message: {message.GetType()}");
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
}
