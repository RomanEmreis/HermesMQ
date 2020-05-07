using HermesMQ;
using HermesMQ.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hermes.Worker.Client {
    public class Program {
        public static void Main(string[] args) {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) => {
                    var loggerFactory = LoggerFactory.Create(builder => {
                        builder
                            .AddFilter("Microsoft", LogLevel.Warning)
                            .AddFilter("System", LogLevel.Warning)
                            .AddFilter("Hermes.Client.Program", LogLevel.Debug)
                            .AddConsole();
                    });

                    var logger = loggerFactory.CreateLogger("client");

                    var configuration = services
                        .BuildServiceProvider()
                        .GetRequiredService<IConfiguration>();

                    services
                        .AddSingleton(logger)
                        .AddHermes(configuration)
                        .AddHostedService<HermesWorker>();
                });
    }
}
