using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using EulynxLive.Point.Services;
using EulynxLive.Point.Connections;
using EulynxLive.Point.Hubs;
using System.Text.Json.Serialization;

namespace EulynxLive.Point
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // TODO: Add switch mappings for better usability
            //  https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-8.0#switch-mappings
            services.AddControllersWithViews();
            services.AddSignalR()
                .AddJsonProtocol(options => options.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
            services.AddGrpc();
            services.AddGrpcReflection();

            services.AddTransient<IConnectionProvider, GrpcConnectionProvider>();
            services.AddTransient<ConnectionFactory>();
            services.AddSingleton(x => x.GetRequiredService<ConnectionFactory>().CreateConnection(x));

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "rasta-point-web/build";
            });

            try
            {
                services.AddSingleton(x =>
                {
                    var simulateTimout = async () => await Task.Delay(new Random().Next(1, 5) * 1000);
                    return ActivatorUtilities.CreateInstance<Point>(x, simulateTimout);
                });
            }
            catch (Exception e)
            {
                Console.WriteLine($"Usage: --PointSettings:LocalId=<> --PointSettings:LocalRastaId=<> --PointSettings:RemoteId=<> --PointSettings:RemoteEndpoint=<>. {e.Message}");
                Environment.Exit(1);
            }

            _ = services.AddHostedService(provider => provider.GetRequiredService<Point>());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();
            app.UseGrpcWeb();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<PointService>().EnableGrpcWeb();
                endpoints.MapGrpcReflectionService();

                endpoints.MapHub<StatusHub>("/status");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "rasta-point-web";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}
