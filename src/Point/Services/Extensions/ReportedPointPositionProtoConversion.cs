using ProtoDegradedPointPosition = EulynxLive.Point.Proto.PointDegradedPosition;
using ProtoPointPosition = EulynxLive.Point.Proto.PointPosition;
using ProtoAbilityToMove = EulynxLive.Point.Proto.AbilityToMove;

using ReportedPointPosition = EulynxLive.FieldElementSubsystems.Interfaces.GenericPointPosition ;
using DegradedPointPosition = EulynxLive.FieldElementSubsystems.Interfaces.GenericDegradedPointPosition ;
using EulynxLive.FieldElementSubsystems.Interfaces;

namespace Point.Services.Extensions
{
    public static class ReportedPointPositionProtoConversion
    {
        public static ProtoPointPosition ConvertToProtoMessage(this ReportedPointPosition reportedPointPosition) => reportedPointPosition switch
        {
            ReportedPointPosition.Right => ProtoPointPosition.Right,
            ReportedPointPosition.Left => ProtoPointPosition.Left,
            ReportedPointPosition.NoEndPosition => ProtoPointPosition.NoEndPosition,
            ReportedPointPosition.UnintendedPosition => ProtoPointPosition.UnintendedPosition,
            _ => throw new InvalidCastException($"Unable to convert reported point position {reportedPointPosition} to proto enum")
        };

        public static ReportedPointPosition ConvertToReportedPointPosition(this ProtoPointPosition pointPositionMessage) => pointPositionMessage switch
        {
            ProtoPointPosition.Right => ReportedPointPosition.Right,
            ProtoPointPosition.Left => ReportedPointPosition.Left,
            ProtoPointPosition.NoEndPosition => ReportedPointPosition.NoEndPosition,
            ProtoPointPosition.UnintendedPosition => ReportedPointPosition.UnintendedPosition,
            _ => throw new InvalidCastException($"Unable to convert point position message {pointPositionMessage} to reported point position.")
        };
    }


    public static class DegradedPointPositionProtoConversion
    {
        public static ProtoDegradedPointPosition ConvertToProtoMessage(this DegradedPointPosition reportedPointPosition) => reportedPointPosition switch
        {
            DegradedPointPosition.DegradedRight => ProtoDegradedPointPosition.DegradedRight,
            DegradedPointPosition.DegradedLeft => ProtoDegradedPointPosition.DegradedLeft,
            DegradedPointPosition.NotDegraded => ProtoDegradedPointPosition.NotDegraded,
            DegradedPointPosition.NotApplicable => ProtoDegradedPointPosition.NotApplicable,
            _ => throw new InvalidCastException($"Unable to convert reported point position {reportedPointPosition} to proto enum")
        };

        public static DegradedPointPosition ConvertToDegradedPointPosition(this ProtoDegradedPointPosition pointPositionMessage) => pointPositionMessage switch
        {
            ProtoDegradedPointPosition.DegradedRight => DegradedPointPosition.DegradedRight,
            ProtoDegradedPointPosition.DegradedLeft => DegradedPointPosition.DegradedLeft,
            ProtoDegradedPointPosition.NotDegraded => DegradedPointPosition.NotDegraded,
            ProtoDegradedPointPosition.NotApplicable => DegradedPointPosition.NotApplicable,
            _ => throw new InvalidCastException($"Unable to convert point position message {pointPositionMessage} to reported point position.")
        };
    }

    public static class AbilityToMoveConversion
    {
        public static GenericAbilityToMove ConvertToGenericAbilityToMove(this ProtoAbilityToMove protoAbilityToMove) => protoAbilityToMove switch
        {
            ProtoAbilityToMove.AbleToMove => GenericAbilityToMove.AbleToMove,
            ProtoAbilityToMove.UnableToMove => GenericAbilityToMove.UnableToMove,
            ProtoAbilityToMove.Undefined => GenericAbilityToMove.Unknown,
            _ => throw new InvalidCastException($"Unable to convert proto ability to move {protoAbilityToMove} to reported point position.")
        };
    }
}
