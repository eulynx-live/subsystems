using System.Threading.Tasks;
using EulynxLive.TrainDetectionSystem.Proto;
using Grpc.Core;
using static EulynxLive.TrainDetectionSystem.Proto.TrainDetectionSystem;

namespace EulynxLive.TrainDetectionSystem.Services
{
    public class TrainDetectionSystemService : TrainDetectionSystemBase {
        private readonly TrainDetectionSystem _tds;

        public TrainDetectionSystemService(TrainDetectionSystem tds)
        {
            _tds = tds;
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
    }
}
