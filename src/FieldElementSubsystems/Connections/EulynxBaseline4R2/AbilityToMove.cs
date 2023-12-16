
using EulynxLive.FieldElementSubsystems.Interfaces;
using EulynxLive.Messages.Baseline4R2;

namespace EulynxLive.FieldElementSubsystems.Connections.EulynxBaseline4R2;

public record AbilityToMove(GenericPointState State) : SpecificAbilityToMove<
    PointAbilityToMovePointMessageReportedAbilityToMovePointStatus
>(State)
{
    public override PointAbilityToMovePointMessageReportedAbilityToMovePointStatus MapInterfacePointPositionToConcrete(GenericAbilityToMove value) => value switch
    {
        GenericAbilityToMove.AbleToMove => PointAbilityToMovePointMessageReportedAbilityToMovePointStatus.PointIsAbleToMove,
        GenericAbilityToMove.UnableToMove => PointAbilityToMovePointMessageReportedAbilityToMovePointStatus.PointIsUnableToMove,
        _ => throw new NotImplementedException(),
    };
}
