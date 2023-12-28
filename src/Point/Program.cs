using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EulynxLive.Point
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    var switchMappings = new Dictionary<string, string>()
                    {
                        { "--technical-identifier", "PointSettings:LocalId" },
                        { "--rasta-id", "PointSettings:LocalRastaId" },
                        { "--remote-identifier", "PointSettings:RemoteId" },
                        { "--remote-endpoint", "PointSettings:RemoteEndpoint" },
                        { "--connection-protocol", "PointSettings:ConnectionProtocol" },
                        { "--all-point-machines-crucial", "PointSettings:AllPointMachinesCrucial" },
                        { "--observe-ability-to-move", "PointSettings:ObserveAbilityToMove" },
                        { "--initial-last-commanded-point-position", "PointSettings:InitialLastCommandedPointPosition" },
                        { "--initial-point-position", "PointSettings:InitialPointPosition" },
                        { "--initial-degraded-point-position", "PointSettings:InitialDegradedPointPosition" },
                        { "--initial-ability-to-move", "PointSettings:InitialAbilityToMove" },
                    };
                    webBuilder
                        .ConfigureAppConfiguration((hostingContext, config) =>
                        {
                            config.AddCommandLine(args, switchMappings);
                        })
                        .UseUrls("http://0.0.0.0:5101")
                        .UseStartup<Startup>();
                });
    }
}
