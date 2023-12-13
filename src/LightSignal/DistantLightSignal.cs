using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;
using EulynxLive.Messages.Deprecated;

namespace EulynxLive.LightSignal
{
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
