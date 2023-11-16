using System.Threading.Tasks;
using Grpc.Core;
using static EulynxLive.Point.Proto.Point;
using EulynxLive.Point.Proto;
using Point.Services.Extensions;

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

        public override async Task<Nothing> SetDegraded(PointDegradedMessage request, ServerCallContext context)
        {
            await _point.SetDegraded(request);
            return new Nothing();
        }

        public override async Task<Nothing> FinalizePosition(Nothing request, ServerCallContext context)
        {
            await _point.FinalizePosition();
            return new Nothing();
        }

        public override Task<Proto.PointPositionMessage> GetPointPosition(Nothing request, ServerCallContext context)
        {
            var response = new Proto.PointPositionMessage()
            {
                Position = _point.PointState.PointPosition.ConvertToProtoMessage()
            };

            return Task.FromResult(response);
        }

        public override Task<SetPointMachineStateResponse> SetPointMachineState(PointMachineStateMessage request, ServerCallContext context)
        {
            _point.PointState.AbilityToMove = request.AbilityToMove;
            _point.PointState.Crucial = request.Crucial;
            _point.PointState.LastPointPosition = request.LastPointPosition;
            _point.PointState.PointPosition = request.PointPosition.ConvertToReportedPointPosition();
            _point.PointState.Target = request.Target;

            return Task.FromResult(new SetPointMachineStateResponse()
            {
                NewState = request,
                Success = true,
            });
        }

        public override Task<PointMachineStateMessage> GetPointMachineState(Nothing request, ServerCallContext context)
        {
            return Task.FromResult(new PointMachineStateMessage()
            {
                AbilityToMove = _point.PointState.AbilityToMove,
                Crucial = _point.PointState.Crucial,
                LastPointPosition = _point.PointState.LastPointPosition,
                PointPosition = _point.PointState.PointPosition.ConvertToProtoMessage(),
                Target = _point.PointState.Target,
            });
        }
    }
}
