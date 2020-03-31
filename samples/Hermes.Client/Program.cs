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

            var connectionFactory = new DefaultHermesConnectionFactory(logger);
            var adapter = new DefaultMessageAdapter();

            await CreateConsumer(connectionFactory, adapter, logger);
            await CreateProducer(connectionFactory, adapter, logger);
        }

        public async static Task CreateProducer(IConnectionFactory connectionFactory, IMessageAdapter adapter, ILogger logger) {
            var connection = await connectionFactory.ConnectAsync("127.0.0.1", 8087);
            var channelWriter = connection.GetOrCreateOutputChannel("client output channel");

            var producer = new DefaultHermesProducer<Payload>(adapter, logger);

            while (true) {
                var payload = new Payload { Data = Console.ReadLine() };
                await producer.ProduceAsync(channelWriter, payload);

                Console.WriteLine();
            }
        }

        public async static Task CreateConsumer(IConnectionFactory connectionFactory, IMessageAdapter adapter, ILogger logger) {
            var connection = await connectionFactory.ConnectAsync("127.0.0.1", 8087);
            var channelWriter = connection.GetOrCreateInputChannel("client input channel");

            var consumer = new DefaultHermesConsumer<Payload>(adapter, logger);

            consumer.OnMessageReceived += (channel, message) => {
                Console.WriteLine($"broadcasting message received {message.Data}");
            };

            _ = consumer.ConsumeAsync(channelWriter);
        }
    }

    class Payload {
        public string Data { get; set; }
    }
}
