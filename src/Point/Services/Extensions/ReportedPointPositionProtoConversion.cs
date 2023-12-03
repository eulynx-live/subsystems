using EulynxLive.Point.Proto;

using ReportedPointPosition = EulynxLive.FieldElementSubsystems.Interfaces.GenericPointPosition;

namespace Point.Services.Extensions
{
    public static class ReportedPointPositionProtoConversion
    {
        public static PointPosition ConvertToProtoMessage(this ReportedPointPosition reportedPointPosition) => reportedPointPosition switch
        {
            ReportedPointPosition.Right => PointPosition.Right,
            ReportedPointPosition.Left => PointPosition.Left,
            ReportedPointPosition.NoEndPosition => PointPosition.NoEndPosition,
            ReportedPointPosition.UnintendedPosition => PointPosition.UnintendedPosition,
            _ => throw new InvalidCastException($"Unable to convert reported point position {reportedPointPosition} to proto enum")
        };

        public static ReportedPointPosition ConvertToReportedPointPosition(this PointPosition pointPositionMessage) => pointPositionMessage switch
        {
            PointPosition.Right => ReportedPointPosition.Right,
            PointPosition.Left => ReportedPointPosition.Left,
            PointPosition.NoEndPosition => ReportedPointPosition.NoEndPosition,
            PointPosition.UnintendedPosition => ReportedPointPosition.UnintendedPosition,
            _ => throw new InvalidCastException($"Unable to convert point position message {pointPositionMessage} to reported point position.")
        };
    }
}
