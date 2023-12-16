namespace EulynxLive.FieldElementSubsystems.Configuration;

public record PointConfiguration(
    string LocalId,
    uint LocalRastaId,
    string RemoteId,
    string RemoteEndpoint,
    bool AllPointMachinesCrucial = true,
    ConnectionProtocol? ConnectionProtocol = null
);
