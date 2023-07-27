using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using EulynxLive.TrainDetectionSystem.Proto;
using Grpc.Core;
using TrainDetectionSystem.Components.TDS;
using static EulynxLive.TrainDetectionSystem.Proto.TrainDetectionSystem;

namespace EulynxLive.TrainDetectionSystem.Services
{
    public class TrainDetectionSystemService : TrainDetectionSystemBase
    {
        private readonly TrainDetectionSystem _tds;
        public readonly IMapper _mapper;


        public TrainDetectionSystemService(TrainDetectionSystem tds, IMapper mapper)
        {
            _tds = tds;
            _mapper = mapper;
        }

        public override async Task<Nothing> IncreaseAxleCount(TpsCommand request, ServerCallContext context)
        {
            await _tds.IncreaseAxleCount(request.Tps);
            return new Nothing();
        }

        public override async Task<Nothing> DecreaseAxleCount(TpsCommand request, ServerCallContext context)
        {
            await _tds.DecreaseAxleCount(request.Tps);
            return new Nothing();
        }

        public override Task<Proto.TDSStateMessage> GetTDSState(Nothing request, ServerCallContext context)
        {
            var response = _mapper.Map<Proto.TDSStateMessage>(_tds.TdsState);

            return Task.FromResult(response);
        }

        public override Task<SetTDSStateResponse> SetTDSState(TDSStateMessage request, ServerCallContext context)
        {
            var newTDSState = _mapper.Map<TDSState>(request);

            CopyProperties(newTDSState, _tds.TdsState);

            return Task.FromResult(new SetTDSStateResponse()
            {
                NewState = request,
                Success = true,
            });
        }

        static void CopyProperties(object source, object destination)
        {
            var sourceType = source.GetType();
            var destinationType = destination.GetType();

            var properties = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                var propertyName = property.Name;
                var sourceValue = property.GetValue(source);

                var destinationProperty = destinationType.GetProperty(propertyName);
                if (destinationProperty != null && destinationProperty.CanWrite)
                {
                    destinationProperty.SetValue(destination, sourceValue);
                }
            }
        }
    }
}
