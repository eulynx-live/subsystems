using ReportedPointPosition = EulynxLive.Messages.Baseline4R1.PointPointPositionMessageReportedPointPosition;
using PointMachineStateMessage = EulynxLive.Point.Proto.PointMachineStateMessage.Types;

namespace EulynxLive.Point.Components
{
    public class PointMachineState
    {
        public ReportedPointPosition PointPosition { get; set; }
        public PointMachineStateMessage.Target Target { get; set; }
        public PointMachineStateMessage.AbilityToMove AbilityToMove { get; set; }
        public PointMachineStateMessage.LastPointPosition LastPointPosition { get; set; }
        public PointMachineStateMessage.Crucial Crucial { get; set; }
    }
}