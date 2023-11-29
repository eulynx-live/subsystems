
using IPointToInterlockingConnection = EulynxLive.Point.Interfaces.IPointToInterlockingConnection;
using PointPosition = EulynxLive.Point.Interfaces.IPointToInterlockingConnection.PointPosition;
using DegradedPointPosition = EulynxLive.Point.Interfaces.IPointToInterlockingConnection.DegradedPointPosition;

namespace EulynxLive.Point.Eulynx;
public abstract record PointState<PP, DP>
{
    private IPointToInterlockingConnection.PointState _state;
    public PP PointPosition { get => MapInterfacePointPositionToConcrete(_state.PointPosition); }
    public DP DegradedPointPosition { get => MapInterfaceDegradedPointPositionToConcrete(_state.DegradedPointPosition); }

    public PointState(IPointToInterlockingConnection.PointState state)
    {
        _state = state;
    }

    public abstract PP MapInterfacePointPositionToConcrete(PointPosition value);

    public abstract DP MapInterfaceDegradedPointPositionToConcrete(DegradedPointPosition value);
}
