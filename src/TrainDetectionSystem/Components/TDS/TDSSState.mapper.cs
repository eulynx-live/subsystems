using AutoMapper;
using EulynxLive.TrainDetectionSystem.Proto;

public class TDSStateMapper : Profile
{
    public TDSStateMapper()
    {
        CreateMap<TrainDetectionSystem.Components.TDS.TDSState, TDSStateMessage>();
        CreateMap<TDSStateMessage, TrainDetectionSystem.Components.TDS.TDSState>();
    }
}