
using EulynxLive.Messages.Baseline4R2;
using IPointToInterlockingConnection = EulynxLive.Point.Interfaces.IPointToInterlockingConnection;
using PointPosition = EulynxLive.Point.Interfaces.IPointToInterlockingConnection.PointPosition;
using DegradedPointPosition = EulynxLive.Point.Interfaces.IPointToInterlockingConnection.DegradedPointPosition;
using EulynxLive.Point.Eulynx;

namespace EulynxLive.Point.EulynxBaseline4R2;

public record PointState : Eulynx.PointState<PointPointPositionMessageReportedPointPosition, PointPointPositionMessageReportedDegradedPointPosition>
{
    public PointState(IPointToInterlockingConnection.PointState state) : base(state)
    {
    }

    public override PointPointPositionMessageReportedPointPosition MapInterfacePointPositionToConcrete(PointPosition value) => value switch
    {
        IPointToInterlockingConnection.PointPosition.Left => PointPointPositionMessageReportedPointPosition.PointIsInALeftHandPositionDefinedEndPosition,
        IPointToInterlockingConnection.PointPosition.Right => PointPointPositionMessageReportedPointPosition.PointIsInARightHandPositionDefinedEndPosition,
        IPointToInterlockingConnection.PointPosition.UnintendetPosition => PointPointPositionMessageReportedPointPosition.PointIsInUnintendedPosition,
        IPointToInterlockingConnection.PointPosition.NoEndposition => PointPointPositionMessageReportedPointPosition.PointIsInNoEndPosition,
        _ => throw new NotImplementedException(),
    };

    public override PointPointPositionMessageReportedDegradedPointPosition MapInterfaceDegradedPointPositionToConcrete(DegradedPointPosition value) => value switch
    {
        IPointToInterlockingConnection.DegradedPointPosition.DegradedLeft => PointPointPositionMessageReportedDegradedPointPosition.PointIsInADegradedLeftHandPosition,
        IPointToInterlockingConnection.DegradedPointPosition.DegradedRight => PointPointPositionMessageReportedDegradedPointPosition.PointIsInADegradedRightHandPosition,
        IPointToInterlockingConnection.DegradedPointPosition.NotDegraded => PointPointPositionMessageReportedDegradedPointPosition.PointIsNotInADegradedPosition,
        IPointToInterlockingConnection.DegradedPointPosition.NotApplicable => PointPointPositionMessageReportedDegradedPointPosition.DegradedPointPositionIsNotApplicable,
        _ => throw new NotImplementedException(),
    };
}
