using EulynxLive.FieldElementSubsystems.Configuration;
using EulynxLive.FieldElementSubsystems.Interfaces;
using EulynxLive.Point.Proto;

using Google.Protobuf.WellKnownTypes;

namespace EulynxLive.Point
{
    public interface IPoint {
        ConnectionProtocol? ConnectionProtocol { get; }
        IPointToInterlockingConnection? Connection { get; }
        GenericPointState PointState { get; }
        void EnableInitializationTimeout(bool enableInitializationTimeout);
        void PreventLeftEndPosition(PreventedPositionMessage request);
        void PreventRightEndPosition(PreventedPositionMessage request);
        Task PutIntoUnintendedPosition(DegradedPositionMessage request);
        Task PutIntoNoEndPosition(DegradedPositionMessage simulatedPositionMessage);
        void Reset();
        Task SendSciMessage(SciMessage request);
        Task SetAbilityToMove(AbilityToMoveMessage request);
        Task SendSciMessagePDIError(Empty request);
        Task SendSciMessageContentError(Empty request);
        Task SendSciMessageFormalError(Empty request);
    }
}
