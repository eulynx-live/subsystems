using EulynxLive.FieldElementSubsystems.Configuration;
using EulynxLive.FieldElementSubsystems.Interfaces;

using EulynxBaseline4R1 = EulynxLive.FieldElementSubsystems.Connections.EulynxBaseline4R1;
using EulynxBaseline4R2 = EulynxLive.FieldElementSubsystems.Connections.EulynxBaseline4R2;

namespace EulynxLive.Point.Connections;

public class ConnectionFactory{
    private IConfiguration _configuration { get; }
    private ILogger<ConnectionFactory> _logger { get; }

    public ConnectionFactory(ILogger<ConnectionFactory> logger, IConfiguration configuration){
        _logger = logger;
        _configuration = configuration;
    }

    public IPointToInterlockingConnection CreateConnection(IServiceProvider x) {
        var connectionProtocol = _configuration.GetSection("ConnectionSettings").Get<PointConfiguration>()?.ConnectionProtocol;
        switch (connectionProtocol){
            case ConnectionProtocol.EulynxBaseline4R1:
                return new EulynxBaseline4R1.PointToInterlockingConnection(x.GetRequiredService<ILogger<EulynxBaseline4R1.PointToInterlockingConnection>>(), _configuration);
            case ConnectionProtocol.EulynxBaseline4R2:
                return new EulynxBaseline4R2.PointToInterlockingConnection(x.GetRequiredService<ILogger<EulynxBaseline4R2.PointToInterlockingConnection>>(), _configuration, CancellationToken.None);
            default:
                if (connectionProtocol != null)
                    _logger.LogWarning($"Unknown connection protocol {connectionProtocol}. Using EulynxBaseline4R2.");
                else
                    _logger.LogWarning($"No connection protocol specified. Using EulynxBaseline4R2.");
                return new EulynxBaseline4R2.PointToInterlockingConnection(x.GetRequiredService<ILogger<EulynxBaseline4R2.PointToInterlockingConnection>>(), _configuration, CancellationToken.None);
        }
    }
}
