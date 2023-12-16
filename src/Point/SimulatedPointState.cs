using EulynxLive.FieldElementSubsystems.Interfaces;
using EulynxLive.Point.Proto;

namespace EulynxLive.Point
{
    public record SimulatedPointState(
        PreventedPosition PreventedPosition,
        GenericDegradedPointPosition DegradedPointPosition,
        bool SimulateTimeout
    );
}
