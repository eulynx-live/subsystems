using EulynxLive.FieldElementSubsystems.Interfaces;

namespace EulynxLive.FieldElementSubsystems.Configuration;

public record PointConfiguration(
    string LocalId,
    uint LocalRastaId,
    string RemoteId,
    string RemoteEndpoint,
    bool AllPointMachinesCrucial = true,
    GenericPointPosition? InitialLastCommandedPointPosition = GenericPointPosition.NoEndPosition,
    GenericPointPosition InitialPointPosition = GenericPointPosition.NoEndPosition,
    GenericDegradedPointPosition InitialDegradedPointPosition = GenericDegradedPointPosition.NotApplicable,
    GenericAbilityToMove InitialAbilityToMove = GenericAbilityToMove.AbleToMove,
    ConnectionProtocol? ConnectionProtocol = null
);
