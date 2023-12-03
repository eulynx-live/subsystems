
using EulynxLive.Messages.Baseline4R1;
using IPointToInterlockingConnection = EulynxLive.Point.Interfaces.IPointToInterlockingConnection;
using PointState = EulynxLive.Point.Interfaces.IPointToInterlockingConnection.PointState;
using PointPosition = EulynxLive.Point.Interfaces.IPointToInterlockingConnection.PointPosition;
using DegradedPointPosition = EulynxLive.Point.Interfaces.IPointToInterlockingConnection.DegradedPointPosition;

public record PointStateBaseline4R1
{
    private PointState _state;
    public PointPointPositionMessageReportedPointPosition PointPosition { get => MapInterfacePointPositionToConcrete(_state.PointPosition); }
    public PointPointPositionMessageReportedDegradedPointPosition DegradedPointPosition { get => MapInterfaceDegradedPointPositionToConcrete(_state.DegradedPointPosition); }

    public PointStateBaseline4R1(PointState state)
    {
        _state = state;
    }

    private PointPointPositionMessageReportedPointPosition MapInterfacePointPositionToConcrete(PointPosition value) => value switch
    {
        IPointToInterlockingConnection.PointPosition.Left => PointPointPositionMessageReportedPointPosition.PointIsInALeftHandPositionDefinedEndPosition,
        IPointToInterlockingConnection.PointPosition.Right => PointPointPositionMessageReportedPointPosition.PointIsInARightHandPositionDefinedEndPosition,
        IPointToInterlockingConnection.PointPosition.UnintendedPosition => PointPointPositionMessageReportedPointPosition.PointIsTrailed,
        IPointToInterlockingConnection.PointPosition.NoEndPosition => PointPointPositionMessageReportedPointPosition.PointIsInNoEndPosition,
        _ => throw new NotImplementedException(),
    };

    private PointPointPositionMessageReportedDegradedPointPosition MapInterfaceDegradedPointPositionToConcrete(DegradedPointPosition value) => value switch
    {
        IPointToInterlockingConnection.DegradedPointPosition.DegradedLeft => PointPointPositionMessageReportedDegradedPointPosition.PointIsInADegradedLeftHandPosition,
        IPointToInterlockingConnection.DegradedPointPosition.DegradedRight => PointPointPositionMessageReportedDegradedPointPosition.PointIsInADegradedRightHandPosition,
        IPointToInterlockingConnection.DegradedPointPosition.NotDegraded => PointPointPositionMessageReportedDegradedPointPosition.PointIsNotInADegradedPosition,
        IPointToInterlockingConnection.DegradedPointPosition.NotApplicable => PointPointPositionMessageReportedDegradedPointPosition.DegradedPointPositionIsNotApplicable,
        _ => throw new NotImplementedException(),
    };
}
