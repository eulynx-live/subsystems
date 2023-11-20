using System.Threading.Tasks;
using Grpc.Core;
using static EulynxLive.Point.Proto.Point;
using EulynxLive.Point.Proto;
using Point.Services.Extensions;
using Google.Protobuf.WellKnownTypes;
using System;

namespace EulynxLive.Point.Services
{
    public class PointService : PointBase
    {
        private readonly Point _point;

        public PointService(Point point)
        {
            _point = point;
        }

        public override async Task<Empty> SimulateUnintendedPosition(Empty request, ServerCallContext context)
        {
            await _point.SimulateUnintendedPosition();
            return new Empty();
        }

        public override async Task<Empty> SetToDegradedPosition(PointDegradedMessage request, ServerCallContext context)
        {
            await _point.SetDegraded(request);
            return new Empty();
        }

        public override async Task<Empty> PutInEndPosition(Empty request, ServerCallContext context)
        {
            await _point.PutInEndPosition();
            return new Empty();
        }

        public override Task<PointPositionMessage> GetPointPosition(Empty request, ServerCallContext context)
        {
            var response = new PointPositionMessage()
            {
                Position = _point.PointState.PointPosition.ConvertToProtoMessage()
            };

            return Task.FromResult(response);
        }

        public override Task<Empty> EstablishPointMachineState(PointMachineStateMessage request, ServerCallContext context)
        {
            // Perform validation
            if (request.Crucial == PointMachineStateMessage.Types.Crucial.Crucial && !_point.AllPointMachinesCrucial) {
                throw new InvalidOperationException("Point has only crucial point machines");
            }

            _point.PointState.AbilityToMove = request.AbilityToMove;
            _point.PointState.LastPointPosition = request.LastPointPosition;
            _point.PointState.PointPosition = request.PointPosition.ConvertToReportedPointPosition();
            _point.PointState.Target = request.Target;

            return Task.FromResult(new Empty());
        }

        public override Task<PointMachineStateMessage> GetPointMachineState(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new PointMachineStateMessage()
            {
                AbilityToMove = _point.PointState.AbilityToMove,
                Crucial = _point.AllPointMachinesCrucial ? PointMachineStateMessage.Types.Crucial.Crucial : PointMachineStateMessage.Types.Crucial.NonCrucial,
                LastPointPosition = _point.PointState.LastPointPosition,
                PointPosition = _point.PointState.PointPosition.ConvertToProtoMessage(),
                Target = _point.PointState.Target,
            });
        }
    }
}
