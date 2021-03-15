using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ordering.Infrastructure.Data;
using System;
using System.Threading.Tasks;

namespace Ordering.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //Build Run Startup ConfigureServices
            var host = CreateHostBuilder(args).Build();

            await CreateAndSeedDatabase(host);
            //Run Async, Run Startup Configure
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }).UseDefaultServiceProvider(options =>
                    options.ValidateScopes = false);

        private static async Task CreateAndSeedDatabase(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            try
            {
                var orderContext = services.GetRequiredService<OrderContext>();
                await OrderContextSeed.SeedAsync(orderContext, loggerFactory);
            }
            catch (Exception exception)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(exception.Message);
            }
        }
    }
}
