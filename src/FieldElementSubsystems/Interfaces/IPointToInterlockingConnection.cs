using EulynxLive.FieldElementSubsystems.Configuration;

namespace EulynxLive.FieldElementSubsystems.Interfaces;

public interface IPointToInterlockingConnection {
    PointConfiguration Configuration { get; }
    CancellationToken TimeoutToken { get; }

    void Connect(IConnection connection);

    Task SendPointPosition(GenericPointState state);
    Task SendSciMessage(byte[] message);
    Task OverrideNextSciMessage(byte[] message);
    Task SendTimeoutMessage();
    Task<bool> InitializeConnection(GenericPointState state, bool observeAbilityToMove, CancellationToken cancellationToken);
    Task<GenericPointPosition> ReceiveMovePointCommand(CancellationToken stoppingToken);
    Task SendAbilityToMove(GenericPointState pointState);
}
