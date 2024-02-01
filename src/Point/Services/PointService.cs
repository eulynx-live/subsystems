using Grpc.Core;
using static EulynxLive.Point.Proto.Point;
using EulynxLive.Point.Proto;
using Point.Services.Extensions;
using Google.Protobuf.WellKnownTypes;
using EulynxLive.FieldElementSubsystems.Configuration;

namespace EulynxLive.Point.Services
{
    public class PointService : PointBase
    {
        private readonly IPoint _point;

        public PointService(IPoint point)
        {
            _point = point;
        }

        public override Task<Empty> Reset(Empty request, ServerCallContext context)
        {
            _point.Reset();
            return Task.FromResult(new Empty());
        }

        public override async Task<Empty> SetAbilityToMove(AbilityToMoveMessage request, ServerCallContext context)
        {
            await _point.SetAbilityToMove(request);
            return new Empty();
        }

        public override async Task<Empty> SendSciMessage(SciMessage request, ServerCallContext context)
        {
            await _point.SendSciMessage(request);
            return new Empty();
        }

        public override async Task<Empty> SendSciMessagePDIError(Empty request, ServerCallContext context)
        {
            await _point.SendSciMessagePDIError(request);
            return new Empty();
        }

        public override async Task<Empty> SendSciMessageContentError(Empty request, ServerCallContext context)
        {
            await _point.SendSciMessageContentError(request);
            return new Empty();
        }

        public override async Task<Empty> SendSciMessageFormalError(Empty request, ServerCallContext context)
        {
            await _point.SendSciMessageFormalError(request);
            return new Empty();
        }

        public override async Task<Empty> OverrideSciMessage(SciMessage request, ServerCallContext context)
        {
            if (_point.Connection == null) throw new InvalidOperationException("Connection is null. Did you call Connect()?");
            await _point.Connection.OverrideNextSciMessage(request.Message.ToByteArray());
            return new Empty();
        }

        public override Task<Empty> ScheduleInitializationTimeout(EnableInitializationTimeoutMessage request, ServerCallContext context)
        {
            _point.EnableInitializationTimeout(request.EnableInitializationTimeout);
            return Task.FromResult(new Empty());
        }

        public override Task<Empty> SchedulePreventLeftEndPosition(PreventedPositionMessage request, ServerCallContext context)
        {
            _point.PreventLeftEndPosition(request);
            return Task.FromResult(new Empty());
        }

        override public Task<Empty> SchedulePreventRightEndPosition(PreventedPositionMessage request, ServerCallContext context)
        {
            _point.PreventRightEndPosition(request);
            return Task.FromResult(new Empty());
        }

        public override async Task<Empty> PutIntoTrailedPosition(DegradedPositionMessage request, ServerCallContext context)
        {
            if (_point.ConnectionProtocol != ConnectionProtocol.EulynxBaseline4R1)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Only supported for EulynxBaseline4R1"));

            await _point.PutIntoUnintendedPosition(request);
            return new Empty();
        }

        public override async Task<Empty> PutIntoUnintendedPosition(DegradedPositionMessage request, ServerCallContext context)
        {
            if (_point.ConnectionProtocol != ConnectionProtocol.EulynxBaseline4R2)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Only supported for EulynxBaseline4R2"));

            await _point.PutIntoUnintendedPosition(request);
            return new Empty();
        }

        public override async Task<Empty> PutIntoNoEndPosition(DegradedPositionMessage request, ServerCallContext context)
        {
            await _point.PutIntoNoEndPosition(request);
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
