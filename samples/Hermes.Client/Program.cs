using Hermes.Abstractions;
using Hermes.Infrastructure.Connection;
using Hermes.Infrastructure.Messaging;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Hermes.Client {
    class Program {
        static async Task Main(string[] args) {
            Console.WriteLine("Press any key for connect");
            Console.ReadKey();

            var loggerFactory = LoggerFactory.Create(builder => {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("Hermes.Client.Program", LogLevel.Debug)
                    .AddConsole();
            });

            var logger = loggerFactory.CreateLogger("client");
            var adapter = new DefaultMessageAdapter();

            var connection = await CreateConsumer(adapter, logger);
            await CreateProducer(adapter, logger);

            connection.Dispose();
        }

        public async static Task CreateProducer(IMessageAdapter adapter, ILogger logger) {
            var connectionFactory = new DefaultHermesConnectionFactory(logger);

            var connection = await connectionFactory.ConnectAsync("127.0.0.1", 8087);
            var channel    = connection.GetOrCreateOutputChannel("client channel");
            var producer   = new DefaultHermesProducer<Guid, Payload>(channel, adapter, logger);

            while (Console.ReadKey().Key != ConsoleKey.Escape) {
                var payload = new Payload { Data = Console.ReadLine() };
                await producer.ProduceAsync(Guid.NewGuid(), payload);

                Console.WriteLine();
            }

            connection.Dispose();
        }

        public async static Task<IConnection> CreateConsumer(IMessageAdapter adapter, ILogger logger) {
            var connectionFactory = new DefaultHermesConnectionFactory(logger);

            var connection = await connectionFactory.ConnectAsync("127.0.0.1", 8087);
            var channel    = connection.GetOrCreateInputChannel("client channel");
            var consumer   = new DefaultHermesConsumer<Guid, Payload>(channel, adapter, logger);

            consumer.OnMessageReceived += (channel, message) => {
                Console.WriteLine($"broadcasting message(key: {message.Key}) received {message.Value.Data}");
            };

            _ = consumer.ConsumeAsync();

            return connection;
        }
    }

    class Payload {
        public string Data { get; set; }
    }
}
