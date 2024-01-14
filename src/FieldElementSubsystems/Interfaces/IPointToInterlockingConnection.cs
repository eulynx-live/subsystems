using EulynxLive.FieldElementSubsystems.Configuration;

namespace EulynxLive.FieldElementSubsystems.Interfaces;

public interface IPointToInterlockingConnectionBuilder {
    IPointToInterlockingConnection Connect(IConnection conn);
}

public interface IPointToInterlockingConnection : IDisposable {
    PointConfiguration Configuration { get; }

    Task SendPointPosition(GenericPointState state);
    Task SendSciMessage(byte[] message);
    Task OverrideNextSciMessage(byte[] message);
    Task SendTimeoutMessage();

    /// <summary>
    /// Initializes the connection. Returns true if the connection was successfully initialized, false otherwise.
    /// </summary>
    /// <param name="state"></param>
    /// <param name="observeAbilityToMove"></param>
    /// <param name="simulateTimeout"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> InitializeConnection(GenericPointState state, bool observeAbilityToMove, bool simulateTimeout, CancellationToken cancellationToken);
    Task<GenericPointPosition> ReceiveMovePointCommand(CancellationToken stoppingToken);
    Task SendAbilityToMove(GenericPointState pointState);
}
