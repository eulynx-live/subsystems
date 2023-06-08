using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TDSStateMessage = EulynxLive.Point.Proto.TDSStateMessage.Types;


namespace TrainDetectionSystem.Components.TDS
{
    public class TDSState
    {
        public TDSStateMessage.AbilityToBeForcedToClear AbilityToBeForcedToClear { get; set; }
        public TDSStateMessage.AbilityToBeForcedToClearBefore AbilityToBeForcedToClearBefore { get; set; }
        public TDSStateMessage.Circuit Circuit { get; set; }
        public TDSStateMessage.ConDetectionOfPassing ConDetectionOfPassing { get; set; }
        public TDSStateMessage.ConDrfc ConDrfc { get; set; }
        public TDSStateMessage.ConMsgAdditionalInformation ConMsgAdditionalInformation { get; set; }
        public TDSStateMessage.ConUfl ConUfl { get; set; }
        public TDSStateMessage.ConUseFcC ConUseFcC { get; set; }
        public TDSStateMessage.ConUseFcP ConUseFcP { get; set; }
        public TDSStateMessage.ConUseFcPA ConUseFcPA { get; set; }
        public TDSStateMessage.ConUseFcU ConUseFcU { get; set; }
        public TDSStateMessage.ConVariant ConVariant { get; set; }
        public TDSStateMessage.DelayOfNotificationOfAvailabilityTimer DelayOfNotificationOfAvailabilityTimer { get; set; }
        public TDSStateMessage.DifferenceBetweenIncomingOutgoingWheels DifferenceBetweenIncomingOutgoingWheels { get; set; }
        public TDSStateMessage.DirectionOfPassing DirectionOfPassing { get; set; }
        public TDSStateMessage.InhibitionTimer InhibitionTimer { get; set; }
        public TDSStateMessage.MsgAdditionalInformation MsgAdditionalInformation { get; set; }
        public TDSStateMessage.PomStatus PomStatus { get; set; }
        public TDSStateMessage.PowerSupply PowerSupply { get; set; }
        public TDSStateMessage.State State { get; set; }
        public TDSStateMessage.StateOfPassing StateOfPassing { get; set; }
        public TDSStateMessage.TvpsOccupancyStatus TvpsOccupancyStatus { get; set; }
        public TDSStateMessage.TvpsOccupancyStatusBefore TvpsOccupancyStatusBefore { get; set; }
        public TDSStateMessage.TvpsOccupiedByAtLeastOneWheel TvpsOccupiedByAtLeastOneWheel { get; set; }
        public TDSStateMessage.TvpsReasonForDisturbance TvpsReasonForDisturbance { get; set; }
        public TDSStateMessage.PdiConection PdiConection { get; set; }
    }
}