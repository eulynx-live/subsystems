using System;
using System.Net.WebSockets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EulynxLive.LightSignal
{
    enum SignalTypeTypes
    {
        main,
        mainShunting,
        multiSection,
        multiSectionShunting,
        shunting,
        distant,
        repeater,
        trainProtection
    }

    public class LightSignalFactory
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EulynxLightSignal> _logger;

        public LightSignalFactory(ILogger<EulynxLightSignal> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public EulynxLightSignal getInstance()
        {

            var signalType = _configuration["signal_type"];
            if (signalType == null)
            {
                throw new Exception("Missing --signal_type command line parameter.");
            }

            var _signalType = Enum.Parse(typeof(SignalTypeTypes), signalType);

            switch (_signalType)
            {
                case SignalTypeTypes.multiSection:
                    return new MultiSectionLightSignal(_logger, _configuration);
                case SignalTypeTypes.distant:
                    return new DistantLightSignal(_logger, _configuration);
                default:
                    return new MultiSectionLightSignal(_logger, _configuration);
            }
        }
    }
}
