using EulynxLive.FieldElementSubsystems.Configuration;

namespace EulynxLive.FieldElementSubsystems.Interfaces;

public interface IPointToInterlockingConnection {
    PointConfiguration Configuration { get; }
    CancellationToken TimeoutToken { get; }

    void Connect(IConnection connection);

    Task SendPointPosition(GenericPointState state);
    Task SendSciMessage(byte[] state);
    Task SendTimeoutMessage();
    Task<bool> InitializeConnection(GenericPointState state, CancellationToken cancellationToken);
    Task<GenericPointPosition?> ReceivePointPosition(CancellationToken stoppingToken);
}
