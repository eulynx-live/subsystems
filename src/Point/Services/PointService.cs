using System.Threading.Tasks;
using Grpc.Core;
using EulynxLive.Messages;
using static EulynxLive.Point.Proto.Point;
using EulynxLive.Point.Proto;
using Proto = EulynxLive.Point.Proto;

namespace EulynxLive.Point.Services {
    public class PointService : PointBase {
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
            switch (_point.Position) {
                case ReportedPointPosition.Right:
                    response.Position = PointPosition.Right;
                    break;
                case ReportedPointPosition.Left:
                    response.Position = PointPosition.Left;
                    break;
                case ReportedPointPosition.Trailed:
                    response.Position = PointPosition.Trailed;
                    break;
                case ReportedPointPosition.NoEndPosition:
                    response.Position = PointPosition.NoEndPosition;
                    break;
            }

            return Task.FromResult(response);
        }
    }
}
