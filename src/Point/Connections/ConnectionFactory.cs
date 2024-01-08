using EulynxLive.FieldElementSubsystems.Configuration;
using EulynxLive.FieldElementSubsystems.Interfaces;

using EulynxBaseline4R1 = EulynxLive.FieldElementSubsystems.Connections.EulynxBaseline4R1;
using EulynxBaseline4R2 = EulynxLive.FieldElementSubsystems.Connections.EulynxBaseline4R2;

namespace EulynxLive.Point.Connections;

public class ConnectionFactory{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ConnectionFactory> _logger;

    public ConnectionFactory(ILogger<ConnectionFactory> logger, IConfiguration configuration){
        _logger = logger;
        _configuration = configuration;
    }

    public IPointToInterlockingConnectionBuilder CreateConnection(IServiceProvider x) {
        var connectionProtocol = _configuration.GetSection("PointSettings").Get<PointConfiguration>()?.ConnectionProtocol;
        switch (connectionProtocol){
            case ConnectionProtocol.EulynxBaseline4R1:
                return ActivatorUtilities.CreateInstance<EulynxBaseline4R1.PointToInterlockingConnectionBuilder>(x, _configuration, CancellationToken.None);
            case ConnectionProtocol.EulynxBaseline4R2:
                return ActivatorUtilities.CreateInstance<EulynxBaseline4R2.PointToInterlockingConnectionBuilder>(x, _configuration, CancellationToken.None);
            case null:
                _logger.LogWarning($"No connection protocol specified. Using EulynxBaseline4R2.");
                return ActivatorUtilities.CreateInstance<EulynxBaseline4R2.PointToInterlockingConnectionBuilder>(x, _configuration, CancellationToken.None);
            default:
                throw new NotImplementedException($"Unknown connection protocol {connectionProtocol}.");
        }
    }
}
