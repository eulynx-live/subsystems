using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EulynxLive.TrainDetectionSystem
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

                    webBuilder
                        .ConfigureAppConfiguration((hostingContext, config) => {
                            config.AddCommandLine(args);
                        })
                        .UseUrls("http://0.0.0.0:5101")
                        .UseStartup<Startup>();
                });
    }
}
