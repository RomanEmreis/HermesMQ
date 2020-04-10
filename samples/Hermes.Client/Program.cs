using Hermes.Abstractions;
using Hermes.Infrastructure.Connection;
using Hermes.Infrastructure.Extensions;
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

            var connection = await CreateConsumer(logger);
            await CreateProducer(logger);

            connection.Dispose();
        }

        public async static Task CreateProducer(ILogger logger) {
            var connectionFactory = new HermesConnectionFactory(logger);

            var connection = await connectionFactory.ConnectAsync("127.0.0.1", 8087);
            var producer   = connection.GetProducer<Guid, Payload>("client channel 1");

            producer.MessageSent += message => {
                Console.WriteLine($"The Message (key: {message.Key}) has been sent to HermesMQ channel {message.ChannelName}");
            };

            while (Console.ReadKey().Key != ConsoleKey.Escape) {
                Console.Write("Write a message: ");

                var payload = new Payload { Data = Console.ReadLine() };
                await producer.ProduceAsync(Guid.NewGuid(), payload);

                Console.WriteLine();
            }

            connection.Dispose();
        }

        public async static Task<IConnection> CreateConsumer(ILogger logger) {
            var connectionFactory = new HermesConnectionFactory(logger);

            var connection = await connectionFactory.ConnectAsync("127.0.0.1", 8087);
            var consumer   = connection.GetConsumer<Guid, Payload>("client channel 1");

            consumer.MessageReceived += (channel, message) => {
                Console.WriteLine($"Received the message (key: {message.Key}) for {message.ChannelName}");
                Console.WriteLine($"Message: {message.Value.Data}");
            };

            _ = consumer.ConsumeAsync();

            return connection;
        }
    }

    class Payload {
        public string Data { get; set; }
    }
}
