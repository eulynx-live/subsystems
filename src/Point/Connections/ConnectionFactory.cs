using EulynxLive.FieldElementSubsystems.Interfaces;

namespace EulynxLive.Point.Connections;

public static class ConnectionFactory{
    public static IPointToInterlockingConnection CreateConnection<T> (IServiceProvider x) where T : IPointToInterlockingConnection {

        return ActivatorUtilities.CreateInstance<T>(x, CancellationToken.None);
    }
}
