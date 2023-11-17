using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EulynxLive.Point.Services;
using EulynxLive.Point.Components;
using System;

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
            services.AddControllersWithViews();
            services.AddGrpc();
            services.AddGrpcReflection();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "rasta-point-web/build";
            });

            try {
                services.AddSingleton<Point>();
            } catch (Exception e) {
                Console.WriteLine($"Usage: --PointSettings:LocalId=<> --PointSettings:LocalRastaId=<> --PointSettings:RemoteId=<> --PointSettings:RemoteEndpoint=<>. {e.Message}");
                Environment.Exit(1);
            }

            services.AddSingleton<PointMachineState>();
            _ = services.AddHostedService(provider => provider.GetService<Point>());
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

            // For live updating the Signal UI.
            app.UseWebSockets();
            // Sends the websockets to the Simulator.
            app.UseMiddleware<WebsocketDispatcherMiddleware>();

            app.UseGrpcWeb();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<PointService>().EnableGrpcWeb();
                if (env.IsDevelopment())
                {
                    endpoints.MapGrpcReflectionService();
                }
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
