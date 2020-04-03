using Hermes.MessageQueue.Service.Application.Extensions;
using Microsoft.Extensions.Hosting;

namespace Hermes.MessageQueue.Service {
    public class Program {
        public static void Main(string[] args) => 
            CreateHostBuilder(args)
                .Build()
                .Run();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) => {
                    services
                        .AddHermesServices()
                        .ConfigureHermesHosting();
                });
    }
}
