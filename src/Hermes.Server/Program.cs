using Hermes.Abstractions;
using Hermes.Infrastructure.Hosting;
using Hermes.Infrastructure.Messaging;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Hermes.Server {
    class Program {
        static async Task Main(string[] args) {
            var loggerFactory = LoggerFactory.Create(builder => {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("Hermes.Server.Program", LogLevel.Debug)
                    .AddConsole();
            });
            var logger = loggerFactory.CreateLogger("server");
            var adapter = new DefaultMessageAdapter();

            await Task.Run(() => StartHost(adapter, logger));

            Console.ReadKey();
        }

        static async Task StartHost(IMessageAdapter adapter, ILogger logger) {
            var host = new DefaultHermesHost(logger);
            await host.StartListenAsync("127.0.0.1", 8087);

            var dispatcher = new DefaultHermesMessageDispatcher(adapter, logger);
            await dispatcher.DispatchAsync();

            host.ClientConnected += newConnection => {
                dispatcher.AddConnection(newConnection);
            };
        }
    }
}
