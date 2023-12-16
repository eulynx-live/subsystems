using EulynxLive.FieldElementSubsystems.Configuration;
using EulynxLive.FieldElementSubsystems.Interfaces;
using EulynxLive.Point.Connections;

using Grpc.Core;

namespace EulynxLive.Point
{
    public class GrpcConnectionProvider : IConnectionProvider
    {
        public IConnection Connect(PointConfiguration configuration, CancellationToken stoppingToken)
        {
            var metadata = new Metadata { { "rasta-id", configuration.LocalRastaId.ToString() } };
            return new GrpcConnection(metadata, configuration.RemoteEndpoint, stoppingToken);
        }
    }
}
