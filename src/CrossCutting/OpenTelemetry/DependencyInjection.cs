using System;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace CrossCutting.OpenTelemetry
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddTelemetry(this IServiceCollection services, string appName)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            services.AddOpenTelemetryTracing((builder) => builder
                        .AddAspNetCoreInstrumentation()
                        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(appName))
                        .AddOtlpExporter(options =>
                        {
                            options.Endpoint = new Uri("http://collection-sumologic-otelcol.sumologic:4317");
                                
                        }));
            return services;
        }
    }
}