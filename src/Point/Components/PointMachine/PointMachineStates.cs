using ReportedPointPosition = EulynxLive.Messages.Baseline4R1.PointPointPositionMessageReportedPointPosition;

namespace EulynxLive.Point.Components
{
    public class PointMachineState
    {
        public ReportedPointPosition position { get; set; }
        public Target target { get; set; }
        public AbilityToMove abilityToMove { get; set; }
        public LastPointPosition lastPointPosition { get; set; }
    }

    public enum Target
    {
        Left,
        Right
    }

    public enum AbilityToMove
    {
        Able,
        Unable
    }

    public enum LastPointPosition
    {
        None
    }

    public enum Crucial
    {
        Crucial = 1,
        NonCrucial = 0
    }
}