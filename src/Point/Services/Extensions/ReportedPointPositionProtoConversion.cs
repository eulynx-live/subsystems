using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EulynxLive.Messages.Baseline4R1;
using EulynxLive.Point.Proto;
using ReportedPointPosition = EulynxLive.Messages.Baseline4R1.PointPointPositionMessageReportedPointPosition;

namespace Point.Services.Extensions
{
    public static class ReportedPointPositionProtoConversion
    {
        public static PointPosition ConvertToProtoMessage(this ReportedPointPosition reportedPointPosition)
        {
            switch (reportedPointPosition)
            {
                case ReportedPointPosition.PointIsInARightHandPositionDefinedEndPosition:
                    return PointPosition.Right;
                case ReportedPointPosition.PointIsInALeftHandPositionDefinedEndPosition:
                    return PointPosition.Left;
                case ReportedPointPosition.PointIsInNoEndPosition:
                    return PointPosition.NoEndPosition;
                case ReportedPointPosition.PointIsTrailed:
                    return PointPosition.Trailed;
                default:
                    throw new InvalidCastException($"Unable to convert reported point position {reportedPointPosition} to proto enum");
            }
        }

        public static ReportedPointPosition ConvertToReportedPointPosition(this PointPosition pointPositionMessage)
        {
            switch (pointPositionMessage)
            {
                case PointPosition.Right:
                    return ReportedPointPosition.PointIsInARightHandPositionDefinedEndPosition;
                case PointPosition.Left:
                    return ReportedPointPosition.PointIsInALeftHandPositionDefinedEndPosition;
                case PointPosition.NoEndPosition:
                    return ReportedPointPosition.PointIsInNoEndPosition;
                case PointPosition.Trailed:
                    return ReportedPointPosition.PointIsTrailed;
                default:
                    throw new InvalidCastException($"Unable to convert point position message {pointPositionMessage} to reported point position.");
            }
        }
    }
}