namespace EulynxLive.FieldElementSubsystems.Interfaces;

public abstract record SpecificPointState<TPointPosition, TDegradedPointPosition>(GenericPointState State)
{
    public TPointPosition PointPosition => MapInterfacePointPositionToConcrete(State.PointPosition);
    public TDegradedPointPosition DegradedPointPosition => MapInterfaceDegradedPointPositionToConcrete(State.DegradedPointPosition);

    public abstract TPointPosition MapInterfacePointPositionToConcrete(GenericPointPosition value);

    public abstract TDegradedPointPosition MapInterfaceDegradedPointPositionToConcrete(GenericDegradedPointPosition value);
}
