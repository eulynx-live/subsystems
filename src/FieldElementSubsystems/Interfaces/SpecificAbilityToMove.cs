namespace EulynxLive.FieldElementSubsystems.Interfaces;

public abstract record SpecificAbilityToMove<TPointPosition>(GenericPointState State)
{
    public TPointPosition AbilityToMove => MapInterfacePointPositionToConcrete(State.AbilityToMove ?? throw new InvalidOperationException("Ability to move is null."));

    public abstract TPointPosition MapInterfacePointPositionToConcrete(GenericAbilityToMove value);
}
