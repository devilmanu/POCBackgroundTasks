using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrossCutting.ServiceDiscovery;
using Hangfire;
using Hangfire.MemoryStorage;
using JobManager.Services;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
namespace JobManager
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
            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.AllowAnyMethod()
                .AllowAnyHeader()
                .AllowAnyOrigin(); // allow credentials
            }));
            services.AddMassTransit(x =>
            {
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
            services.AddTransient<JobService>();
            services.AddHangfire(config =>
            {
                config = GlobalConfiguration.Configuration.UseSqlServerStorage(Configuration.GetConnectionString("sql"));
            });

            services.AddHttpClient();
            services.AddControllers().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.AddSwaggerGen();
            services.RegisterConsulServices(Configuration.GetSection(nameof(ServiceConfig)).Get<ServiceConfig>());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHangfireDashboard();
            app.UseHangfireServer();

            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Job Manager V1");
            });

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("MyPolicy");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
