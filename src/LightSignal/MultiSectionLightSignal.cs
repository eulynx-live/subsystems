using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using EulynxLive.Messages;
using System.Threading.Channels;

namespace EulynxLive.LightSignal
{
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
}
