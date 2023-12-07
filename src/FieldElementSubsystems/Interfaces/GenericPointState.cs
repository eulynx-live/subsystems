namespace EulynxLive.FieldElementSubsystems.Interfaces;

public record GenericPointState
{
    public GenericPointPosition PointPosition { get; set; }
    public GenericDegradedPointPosition DegradedPointPosition { get; set; }
}
