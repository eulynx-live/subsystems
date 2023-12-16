using EulynxLive.FieldElementSubsystems.Configuration;
using EulynxLive.FieldElementSubsystems.Interfaces;

namespace EulynxLive.Point
{
    public interface IConnectionProvider
    {
        IConnection Connect(PointConfiguration configuration, CancellationToken stoppingToken);
    }
}
