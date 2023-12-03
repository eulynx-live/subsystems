using EulynxLive.FieldElementSubsystems.Configuration;

namespace EulynxLive.FieldElementSubsystems.Interfaces;

public interface IPointToInterlockingConnection : IDisposable {
    PointConfiguration Configuration { get; }
    CancellationToken TimeoutToken { get; }
    Task SendPointPosition(GenericPointState state);
    Task SendTimeoutMessage();
    void Connect(IConnection connection);
    Task<bool> InitializeConnection(GenericPointState state, CancellationToken cancellationToken);
    public Task<GenericPointPosition?> ReceivePointPosition(CancellationToken stoppingToken);
}
