using EulynxLive.FieldElementSubsystems.Configuration;

namespace EulynxLive.FieldElementSubsystems.Interfaces;

public interface IPointToInterlockingConnection {
    PointConfiguration Configuration { get; }
    CancellationToken TimeoutToken { get; }
    Task SendPointPosition(GenericPointState state);
    Task SendGenericMessage(byte[] state);
    Task SendTimeoutMessage();
    Task SendAbilityToMoveMessage(GenericAbiliyToMove abilityToMove);
    void Connect(IConnection connection);
    Task<bool> InitializeConnection(GenericPointState state, CancellationToken cancellationToken);
    public Task<GenericPointPosition?> ReceivePointPosition(CancellationToken stoppingToken);
}
