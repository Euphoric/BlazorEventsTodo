using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorEventsTodo.Server
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var webHost = CreateHostBuilder(args).Build();

            await WaitOnStartup(webHost);

            await Startup.InitializeServices(webHost.Services);

            await webHost.RunAsync();
        }

        private static async Task WaitOnStartup(IHost webHost)
        {
            var configuration = webHost.Services.GetRequiredService<IConfiguration>();
            var waitOnStartup = configuration.GetValue("WaitOnStartup", (int?)null);
            if (waitOnStartup.HasValue)
            {
                var logger = webHost.Services.GetRequiredService<ILoggerFactory>().CreateLogger(typeof(Program));
                logger.LogInformation("Waiting for {seconds}s", waitOnStartup.Value);
                await Task.Delay(TimeSpan.FromSeconds(waitOnStartup.Value));
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
