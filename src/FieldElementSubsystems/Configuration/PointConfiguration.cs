namespace EulynxLive.FieldElementSubsystems.Configuration;

public record PointConfiguration(
    string LocalId,
    int LocalRastaId,
    string RemoteId,
    string RemoteEndpoint,
    bool? AllPointMachinesCrucial = null,
    bool? SimulateRandomTimeouts = null
);