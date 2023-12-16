namespace EulynxLive.FieldElementSubsystems.Interfaces;

public record GenericPointState(
    GenericPointPosition? LastCommandedPointPosition,
    GenericPointPosition PointPosition,
    GenericDegradedPointPosition DegradedPointPosition,
    GenericAbilityToMove? AbilityToMove
);
