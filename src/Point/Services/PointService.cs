using System.Threading.Tasks;
using Grpc.Core;
using EulynxLive.Messages.Baseline4R1;
using static EulynxLive.Point.Proto.Point;
using EulynxLive.Point.Proto;

namespace EulynxLive.Point.Services
{
    public class PointService : PointBase
    {
        private readonly Point _point;

        public PointService(Point point)
        {
            _point = point;
        }

        public override async Task<Nothing> SimulateTrailed(Nothing request, ServerCallContext context)
        {
            await _point.SimulateTrailed();
            return new Nothing();
        }

        public override Task<Proto.PointPositionMessage> GetPointPosition(Nothing request, ServerCallContext context)
        {
            var response = new Proto.PointPositionMessage();
            switch (_point.Position)
            {
                case PointPointPositionMessageReportedPointPosition.PointIsInARightHandPositionDefinedEndPosition:
                    response.Position = PointPosition.Right;
                    break;
                case PointPointPositionMessageReportedPointPosition.PointIsInALeftHandPositionDefinedEndPosition:
                    response.Position = PointPosition.Left;
                    break;
                case PointPointPositionMessageReportedPointPosition.PointIsInNoEndPosition:
                    response.Position = PointPosition.NoEndPosition;
                    break;
                case PointPointPositionMessageReportedPointPosition.PointIsTrailed:
                    response.Position = PointPosition.Trailed;
                    break;
            }

            return Task.FromResult(response);
        }
    }
}
