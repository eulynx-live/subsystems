
using EulynxLive.Messages.Baseline4R1;
using EulynxLive.FieldElementSubsystems.Interfaces;

namespace EulynxLive.FieldElementSubsystems.Connections.EulynxBaseline4R1;

public record PointState(GenericPointState State) : SpecificPointState<
    PointPointPositionMessageReportedPointPosition,
    PointPointPositionMessageReportedDegradedPointPosition
>(State)
{
    public override PointPointPositionMessageReportedPointPosition MapInterfacePointPositionToConcrete(GenericPointPosition value) => value switch
    {
        GenericPointPosition.Left => PointPointPositionMessageReportedPointPosition.PointIsInALeftHandPositionDefinedEndPosition,
        GenericPointPosition.Right => PointPointPositionMessageReportedPointPosition.PointIsInARightHandPositionDefinedEndPosition,
        GenericPointPosition.UnintendedPosition => PointPointPositionMessageReportedPointPosition.PointIsTrailed,
        GenericPointPosition.NoEndPosition => PointPointPositionMessageReportedPointPosition.PointIsInNoEndPosition,
        _ => throw new NotImplementedException(),
    };

    public override PointPointPositionMessageReportedDegradedPointPosition MapInterfaceDegradedPointPositionToConcrete(GenericDegradedPointPosition value) => value switch
    {
        GenericDegradedPointPosition.DegradedLeft => PointPointPositionMessageReportedDegradedPointPosition.PointIsInADegradedLeftHandPosition,
        GenericDegradedPointPosition.DegradedRight => PointPointPositionMessageReportedDegradedPointPosition.PointIsInADegradedRightHandPosition,
        GenericDegradedPointPosition.NotDegraded => PointPointPositionMessageReportedDegradedPointPosition.PointIsNotInADegradedPosition,
        GenericDegradedPointPosition.NotApplicable => PointPointPositionMessageReportedDegradedPointPosition.DegradedPointPositionIsNotApplicable,
        _ => throw new NotImplementedException(),
    };
}
