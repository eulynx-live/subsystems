using EulynxLive.Point.Interfaces;

namespace EulynxLive.Point.Eulynx;

public static class ConnectionFactory{
    public static IPointToInterlockingConnection CreateConnection<T> (IServiceProvider x) where T : IPointToInterlockingConnection {

        if (typeof(T).IsAssignableFrom(typeof(EulynxBaseline4R1.PointToInterlockingConnection)))
        {
            return new EulynxBaseline4R1.PointToInterlockingConnection(x.GetRequiredService<ILogger<EulynxBaseline4R1.PointToInterlockingConnection>>(), x.GetRequiredService<IConfiguration>(), CancellationToken.None);
        }

        if (typeof(T).IsAssignableFrom(typeof(EulynxBaseline4R2.PointToInterlockingConnection)))
        {
            return new EulynxBaseline4R2.PointToInterlockingConnection(x.GetRequiredService<ILogger<EulynxBaseline4R2.PointToInterlockingConnection>>(), x.GetRequiredService<IConfiguration>(), CancellationToken.None);
        }

        throw new NotImplementedException("Trying to create unknown connection.");
    }
}
