

using System;
using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;

namespace ApiGateway
{


    public class Program
    {
        public static IConfiguration _config { get; set; }

        public static void Main(string[] args)
        {
            Activity.DefaultIdFormat = ActivityIdFormat.W3C;
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
             .ConfigureAppConfiguration((hostingContext, config) =>
             {

                 
                 _config = config
                     .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                     .AddJsonFile("ocelot.json")
                     .AddEnvironmentVariables()
                     .Build();
                var uri = _config.GetServiceUri("consul");
                Console.WriteLine($"{uri}");
             })
            .ConfigureServices(services =>
            {
                services.AddCors(options =>
                {
                    options.AddDefaultPolicy(
                        builder =>
                        {
                                builder.AllowAnyOrigin()
                                        .AllowAnyHeader()
                                        .AllowAnyMethod();
                        });
                });
                services.AddOcelot(_config)
                    .AddConsul();
            })
            .Configure(app =>
            {
                app.UseCors();
                app.UseWebSockets();
                app.UseOcelot().Wait();
            });
    }
}
