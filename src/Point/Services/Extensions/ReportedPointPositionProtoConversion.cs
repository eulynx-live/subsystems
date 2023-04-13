using System;
using EulynxLive.Point.Proto;
using ReportedPointPosition = EulynxLive.Messages.Baseline4R1.PointPointPositionMessageReportedPointPosition;

namespace Point.Services.Extensions
{
    public static class ReportedPointPositionProtoConversion
    {
        public static PointPosition ConvertToProtoMessage(this ReportedPointPosition reportedPointPosition) => reportedPointPosition switch
        {
            ReportedPointPosition.PointIsInARightHandPositionDefinedEndPosition => PointPosition.Right,
            ReportedPointPosition.PointIsInALeftHandPositionDefinedEndPosition => PointPosition.Left,
            ReportedPointPosition.PointIsInNoEndPosition => PointPosition.NoEndPosition,
            ReportedPointPosition.PointIsTrailed => PointPosition.Trailed,
            _ => throw new InvalidCastException($"Unable to convert reported point position {reportedPointPosition} to proto enum")
        };

        public static ReportedPointPosition ConvertToReportedPointPosition(this PointPosition pointPositionMessage) => pointPositionMessage switch
        {
            PointPosition.Right => ReportedPointPosition.PointIsInARightHandPositionDefinedEndPosition,
            PointPosition.Left => ReportedPointPosition.PointIsInALeftHandPositionDefinedEndPosition,
            PointPosition.NoEndPosition => ReportedPointPosition.PointIsInNoEndPosition,
            PointPosition.Trailed => ReportedPointPosition.PointIsTrailed,
            _ => throw new InvalidCastException($"Unable to convert point position message {pointPositionMessage} to reported point position.")
        };
    }
}