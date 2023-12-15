using Grpc.Core;
using static EulynxLive.Point.Proto.Point;
using EulynxLive.Point.Proto;
using Point.Services.Extensions;
using Google.Protobuf.WellKnownTypes;

namespace EulynxLive.Point.Services
{
    public class PointService : PointBase
    {
        private readonly Point _point;

        public PointService(Point point)
        {
            _point = point;
        }

        public override async Task<Empty> Reset(Empty request, ServerCallContext context)
        {
            await _point.Reset();
            return new Empty();
        }

        public override async Task<Empty> SendGenericMessage(GenericSCIMessage request, ServerCallContext context)
        {
            await _point.SendSCIMessage(request);
            return new Empty();
        }

        public override async Task<Empty> PreventEndPosition(SimulatedPositionMessage message, ServerCallContext context)
        {
            await _point.PreventEndPosition(message);
            return new Empty();
        }

        public override async Task<Empty> PutInEndPosition(Empty request, ServerCallContext context)
        {
            await _point.PutInEndPosition();
            return new Empty();
        }

        public override Task<PointDegradedPositionMessage> GetDegradedPointPosition(Empty request, ServerCallContext context)
        {
            var response = new PointDegradedPositionMessage()
            {
                Position = _point.PointState.DegradedPointPosition.ConvertToProtoMessage()
            };

            return Task.FromResult(response);
        }

        public override Task<PointPositionMessage> GetPointPosition(Empty request, ServerCallContext context)
        {
            var response = new PointPositionMessage()
            {
                Position = _point.PointState.PointPosition.ConvertToProtoMessage()
            };

            return Task.FromResult(response);
        }

    }
}
