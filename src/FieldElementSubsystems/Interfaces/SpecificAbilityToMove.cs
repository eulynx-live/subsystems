namespace EulynxLive.FieldElementSubsystems.Interfaces;

public abstract record SpecificAbilityToMove<TPointPosition>(GenericPointState State)
{
    public TPointPosition AbilityToMove => MapInterfacePointPositionToConcrete(State.AbilityToMove);

    public abstract TPointPosition MapInterfacePointPositionToConcrete(GenericAbilityToMove value);
}
