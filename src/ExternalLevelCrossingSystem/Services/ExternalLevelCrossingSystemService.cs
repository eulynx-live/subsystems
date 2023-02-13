using System.Threading.Tasks;
using Grpc.Core;
using EulynxLive.Messages;
using static EulynxLive.ExternalLevelCrossingSystem.Proto.ExternalLevelCrossingSystem;
using EulynxLive.ExternalLevelCrossingSystem.Proto;
using Proto = EulynxLive.ExternalLevelCrossingSystem.Proto;

namespace EulynxLive.ExternalLevelCrossingSystem.Services {
    public class ExternalLevelCrossingSystemService : ExternalLevelCrossingSystemBase {
        private readonly ExternalLevelCrossingSystem _externalLevelCrossingSystem;

        public ExternalLevelCrossingSystemService(ExternalLevelCrossingSystem externalLevelCrossingSystem)
        {
            _externalLevelCrossingSystem = externalLevelCrossingSystem;
        }
    }
}
