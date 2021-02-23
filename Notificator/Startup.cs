using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MassTransit;
using MassTransit.SignalR;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;

namespace Notificator
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
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    );

                options.AddPolicy("signalr",
                    builder => builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithOrigins("http://127.0.0.1:5500")
                    .AllowCredentials()
                    .SetIsOriginAllowed(hostName => true));
            });
            services.AddSignalR();
            services.AddMassTransit(x =>
            {
                x.AddSignalRHub<JobHub>();
                x.AddConsumer<CliConsumer>();
                x.SetKebabCaseEndpointNameFormatter();
                x.UsingRabbitMq((context, cfg) => {
                    cfg.ConfigureEndpoints(context);
                    cfg.Host("localhost", (h) =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });
                });
            });
            services.AddMassTransitHostedService();
            services.AddControllers();
            // Adding the OtlpExporter creates a GrpcChannel.
            // This switch must be set before creating a GrpcChannel/HttpClient when calling an insecure gRPC service.
            // See: https://docs.microsoft.com/aspnet/core/grpc/troubleshoot#call-insecure-grpc-services-with-net-core-client
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            services.AddOpenTelemetryTracing((builder) => builder
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(this.Configuration.GetValue<string>("Otlp:ServiceName")))
                .AddAspNetCoreInstrumentation()
                .AddOtlpExporter(otlpOptions =>
                {
                    otlpOptions.Endpoint = new Uri(this.Configuration.GetValue<string>("Otlp:Endpoint"));
                }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            app.UseCors("signalr");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<JobHub>("/jobhub");
            });
        }
    }
}
