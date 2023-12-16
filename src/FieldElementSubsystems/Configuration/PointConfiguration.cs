using EulynxLive.FieldElementSubsystems.Interfaces;

namespace EulynxLive.FieldElementSubsystems.Configuration;

public record PointConfiguration(
    string LocalId,
    uint LocalRastaId,
    string RemoteId,
    string RemoteEndpoint,
    // Whether all point machines are crucial to the point's operation.
    // - Point has only a single point machine or
    // - Point has multiple point machines, but they are all crucial or
    // - Point does not support reporting degraded point positions
    bool AllPointMachinesCrucial = true,
    GenericPointPosition? InitialLastCommandedPointPosition = GenericPointPosition.NoEndPosition,
    GenericPointPosition InitialPointPosition = GenericPointPosition.NoEndPosition,
    GenericDegradedPointPosition InitialDegradedPointPosition = GenericDegradedPointPosition.NotApplicable,
    GenericAbilityToMove InitialAbilityToMove = GenericAbilityToMove.AbleToMove,
    ConnectionProtocol? ConnectionProtocol = null
);
