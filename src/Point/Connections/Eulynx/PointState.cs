
using IPointToInterlockingConnection = EulynxLive.Point.Interfaces.IPointToInterlockingConnection;
using PointPosition = EulynxLive.Point.Interfaces.IPointToInterlockingConnection.PointPosition;
using DegradedPointPosition = EulynxLive.Point.Interfaces.IPointToInterlockingConnection.DegradedPointPosition;

namespace EulynxLive.Point.Eulynx;
public abstract record PointState<TPointPosition, TDegradedPointPosition>(IPointToInterlockingConnection.PointState State)
{
    public TPointPosition PointPosition => MapInterfacePointPositionToConcrete(State.PointPosition);
    public TDegradedPointPosition DegradedPointPosition => MapInterfaceDegradedPointPositionToConcrete(State.DegradedPointPosition);

    public abstract TPointPosition MapInterfacePointPositionToConcrete(PointPosition value);

    public abstract TDegradedPointPosition MapInterfaceDegradedPointPositionToConcrete(DegradedPointPosition value);
}
