using System;
using System.Collections.Generic;
using System.Linq;
using EulynxLive.Messages;
using EulynxLive.Messages.Deprecated;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EulynxLive.LightSignal
{
    public enum SignalTypeTypes
    {
        Main,
        MainShunting,
        MultiSection,
        MultiSectionShunting,
        Shunting,
        Distant,
        Repeater,
        TrainProtection
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

        public class LightSignalConfig
        {
            public string Id { get; set; }
            public SignalTypeTypes Type { get; set; }
        }

        public List<EulynxLightSignal> Create(System.Threading.Channels.Channel<EulynxMessage> outgoingMessages)
        {

            var signalType = _configuration.GetSection("LightSignals").Get<List<LightSignalConfig>>();
            if (signalType == null)
            {
                throw new Exception("Missing light signals configuration.");
            }


            // Command line argument parsing.

            var remoteId = _configuration["remote-id"];
            if (remoteId == null)
            {
                throw new Exception("Missing --remote-id command line parameter.");
            }

            return signalType.Select<LightSignalConfig, EulynxLightSignal>(x => x.Type switch
            {
                SignalTypeTypes.MultiSection => new MultiSectionLightSignal(_logger, x.Id, remoteId, outgoingMessages),
                SignalTypeTypes.Distant => new DistantLightSignal(_logger, x.Id, remoteId, outgoingMessages),
                _ => new MultiSectionLightSignal(_logger, x.Id, remoteId, outgoingMessages),
            }).ToList();
        }
    }
}
