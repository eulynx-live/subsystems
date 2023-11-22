using System;
using EulynxLive.Point.Proto;

using ReportedPointPosition = EulynxLive.Messages.IPointToInterlockingConnection.IPointToInterlockingConnection.PointPosition ;
using DegradedPointPosition = EulynxLive.Messages.IPointToInterlockingConnection.IPointToInterlockingConnection.DegradedPointPosition;

namespace Point.Services.Extensions
{
    public static class ReportedPointPositionProtoConversion
    {
        public static PointPosition ConvertToProtoMessage(this ReportedPointPosition reportedPointPosition) => reportedPointPosition switch
        {
            ReportedPointPosition.RIGHT => PointPosition.Right,
            ReportedPointPosition.LEFT => PointPosition.Left,
            ReportedPointPosition.NO_ENDPOSITION => PointPosition.NoEndPosition,
            ReportedPointPosition.TRAILED => PointPosition.UnintendedPosition,
            _ => throw new InvalidCastException($"Unable to convert reported point position {reportedPointPosition} to proto enum")
        };

        public static ReportedPointPosition ConvertToReportedPointPosition(this PointPosition pointPositionMessage) => pointPositionMessage switch
        {
            PointPosition.Right => ReportedPointPosition.RIGHT,
            PointPosition.Left => ReportedPointPosition.LEFT,
            PointPosition.NoEndPosition => ReportedPointPosition.NO_ENDPOSITION,
            PointPosition.UnintendedPosition => ReportedPointPosition.TRAILED,
            _ => throw new InvalidCastException($"Unable to convert point position message {pointPositionMessage} to reported point position.")
        };
    }
}
