using ReportedPointPosition = EulynxLive.Messages.Baseline4R1.PointPointPositionMessageReportedPointPosition;
using ReportedDegradedPointPosition = EulynxLive.Messages.Baseline4R1.PointPointPositionMessageReportedDegradedPointPosition;
using PointMachineStateMessage = EulynxLive.Point.Proto.PointMachineStateMessage.Types;

namespace EulynxLive.Point.Components
{
    public class PointMachineState
    {
        public ReportedPointPosition PointPosition { get; set; }
        public ReportedDegradedPointPosition DegradedPointPosition { get; set; }
        public PointMachineStateMessage.Target Target { get; set; }
        public PointMachineStateMessage.AbilityToMove AbilityToMove { get; set; }
        public PointMachineStateMessage.LastPointPosition LastPointPosition { get; set; }
        public PointMachineStateMessage.Crucial Crucial { get; set; }
    }
}
