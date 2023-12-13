using System.Threading.Tasks;
using static EulynxLive.LightSignal.Proto.LightSignal;
using EulynxLive.LightSignal.Proto;
using EulynxLive.Messages.Deprecated;

namespace EulynxLive.LightSignal.Services
{
    public class LightSignalService : LightSignalBase {
        private readonly EulynxLightSignal _lightSignal;

        public LightSignalService(EulynxLightSignal LightSignal)
        {
            _lightSignal = LightSignal;
        }

        public override Task<SignalAspectMessage> GetSignalAspect(Nothing request, Grpc.Core.ServerCallContext context)
        {
            var response = new SignalAspectMessage();
            response.Aspect = SignalAspect.StopDanger1;
            switch (_lightSignal.BasicAspectType) {
                case BasicAspectType.Stop_Danger_1:
                    response.Aspect = SignalAspect.StopDanger1;
                    break;
                case BasicAspectType.Proceed_Clear_1:
                    response.Aspect = SignalAspect.ProceedClear1;
                    break;
                case BasicAspectType.ExpectStop:
                    response.Aspect = SignalAspect.ExpectStop;
                    break;
            }

            return Task.FromResult(response);
        }
    }
}
