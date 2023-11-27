using ReportedPointPosition = EulynxLive.Messages.Baseline4R1.PointPointPositionMessageReportedPointPosition;
using ReportedDegradedPointPosition = EulynxLive.Messages.Baseline4R1.PointPointPositionMessageReportedDegradedPointPosition;
using PointMachineStateMessage = EulynxLive.FieldElementPoint.Proto.PointMachineStateMessage.Types;

namespace EulynxLive.FieldElementPoint.Components
{
    public class PointMachineState
    {
        public ReportedPointPosition PointPosition { get; set; }
        public ReportedDegradedPointPosition DegradedPointPosition { get; set; }
        public PointMachineStateMessage.Target Target { get; set; }
        public PointMachineStateMessage.AbilityToMove AbilityToMove { get; set; }
        public PointMachineStateMessage.LastPointPosition LastPointPosition { get; set; }
    }
}
