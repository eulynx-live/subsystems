using EulynxLive.FieldElementSubsystems.Interfaces;
using EulynxLive.Point.Proto;

namespace EulynxLive.Point
{
    public record SimulatedPointState(
        PreventedPosition PreventedPositionLeft,
        PreventedPosition PreventedPositionRight,
        bool DegradedPositionLeft,
        bool DegradedPositionRight,
        bool SimulateTimeoutLeft,
        bool SimulateTimeoutRight
    );
}
