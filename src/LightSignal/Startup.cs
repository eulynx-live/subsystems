using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using EulynxLive.LightSignal.Services;

namespace EulynxLive.LightSignal
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
                configuration.RootPath = "rasta-light-signal-web/build";
            });

            services.AddTransient<LightSignalFactory>();
            services.AddSingleton<EulynxLightSignal>(provider => provider.GetService<LightSignalFactory>().getInstance());

            services.AddHostedService(provider => provider.GetService<EulynxLightSignal>());
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
                endpoints.MapGrpcService<LightSignalService>().EnableGrpcWeb();
                if (env.IsDevelopment())
                {
                    endpoints.MapGrpcReflectionService();
                }
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "rasta-light-signal-web";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}
