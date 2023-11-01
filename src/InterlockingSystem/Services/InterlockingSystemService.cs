using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using EulynxLive.InterlockingSystem.Components;
using EulynxLive.InterlockingSystem.Proto;
using Grpc.Core;
using static EulynxLive.InterlockingSystem.Proto.InterlockingSystem;

namespace EulynxLive.InterlockingSystem.Services
{
    public class InterlockingSystemService : Proto.InterlockingSystem.InterlockingSystemBase
    {
        private readonly InterlockingSystem _interlockingSystem;
        public readonly IMapper _mapper;
        
        public InterlockingSystemService(InterlockingSystem interlockingSystem, IMapper mapper)
        {
            _interlockingSystem = interlockingSystem;
            _mapper = mapper;
        }
        
        public override async Task<Nothing> SetBlock(InterlockingSystemCommand request, ServerCallContext context)
        {
            await _interlockingSystem.SetBlock(request.InterlockingSystem);
            return new Nothing();
        }

        public override async Task<Nothing> UnsetBlock(InterlockingSystemCommand request, ServerCallContext context)
        {
            await _interlockingSystem.UnsetBlock(request.InterlockingSystem);
            return new Nothing();
        }
        
        public override async Task<Nothing> InitiateDirectionHandover(InterlockingSystemCommand request, ServerCallContext context)
        {
            await _interlockingSystem.InitiateDirectionHandover(request.InterlockingSystem);
            return new Nothing();
        }

        public override Task<Proto.InterlockingSystemStateMessage> GetInterlockingSystemState(Nothing request, ServerCallContext context)
        {
            var response = _mapper.Map<Proto.InterlockingSystemStateMessage>(_interlockingSystem.interlockingSystemState);

            return Task.FromResult(response);
        }

        public override Task<SetInterlockingSystemStateResponse> SetInterlockingSystemState(InterlockingSystemStateMessage request, ServerCallContext context)
        {
            var newInterlockingSystemState = _mapper.Map<InterlockingSystemState>(request);

            CopyProperties(newInterlockingSystemState, _interlockingSystem.interlockingSystemState);

            return Task.FromResult(new SetInterlockingSystemStateResponse()
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
