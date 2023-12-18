using EulynxLive.FieldElementSubsystems.Configuration;
using EulynxLive.FieldElementSubsystems.Interfaces;
using EulynxLive.Point.Proto;

namespace EulynxLive.Point
{
    public interface IPoint {
        ConnectionProtocol? ConnectionProtocol { get; }
        IPointToInterlockingConnection Connection { get; }
        GenericPointState PointState { get; }

        void EnableTimeoutLeft(bool enableMovementFailed);
        void EnableTimeoutRight(bool enableMovementFailed);
        void PreventLeftEndPosition(PreventedPositionMessage request);
        void PreventRightEndPosition(PreventedPositionMessage request);
        Task PutIntoUnintendedPosition(DegradedPositionMessage request);
        void Reset();
        Task SendSciMessage(SciMessage request);
        Task SetAbilityToMove(AbilityToMoveMessage request);
    }
}
