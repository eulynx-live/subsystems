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

        public override async Task<Empty> PreventEndPosition(PreventedPositionMessage message, ServerCallContext context)
        {
            _point.PreventEndPosition(message);
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

            _point.PointState.PointPosition = request.PointPosition.ConvertToReportedPointPosition();

            return Task.FromResult(new Empty());
        }

        public override Task<PointMachineStateMessage> GetPointMachineState(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new PointMachineStateMessage()
            {
                Crucial = _point.AllPointMachinesCrucial ? PointMachineStateMessage.Types.Crucial.Crucial : PointMachineStateMessage.Types.Crucial.NonCrucial,
                PointPosition = _point.PointState.PointPosition.ConvertToProtoMessage(),
            });
        }
    }
}
