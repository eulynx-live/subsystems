using System.Threading.Tasks;
using Grpc.Core;
using EulynxLive.Messages;
using static EulynxLive.LevelCrossing.Proto.LevelCrossing;
using EulynxLive.LevelCrossing.Proto;
using Proto = EulynxLive.LevelCrossing.Proto;

namespace EulynxLive.LevelCrossing.Services {
    public class LevelCrossingService : LevelCrossingBase {
        private readonly LevelCrossing _levelCrossing;

        public LevelCrossingService(LevelCrossing levelCrossing)
        {
            _levelCrossing = levelCrossing;
        }
    }
}
