using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using CliWrap;

namespace JobManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Activity.DefaultIdFormat = ActivityIdFormat.W3C;
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    if(args.Contains("seed")){
                        Cli.Wrap("rh").WithArguments("-f /Mitrations -databasename=Jobs --connectionstring=\"User ID=sa;Password=fMJkotp0p.;Database=Jobs;server=localhost;\" -t").ExecuteAsync().GetAwaiter().GetResult();
                    }
                    webBuilder.UseStartup<Startup>();
                });
    }
}
