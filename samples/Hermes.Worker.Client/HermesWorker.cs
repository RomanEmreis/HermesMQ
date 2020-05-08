using System;
using System.Threading;
using System.Threading.Tasks;
using Hermes.Abstractions;
using Hermes.Infrastructure.Extensions;
using Hermes.Worker.Client.Application;
using Hermes.Worker.Client.Models;
using HermesMQ.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Hermes.Worker.Client {
    public class HermesWorker : BackgroundService {
        private readonly IConnectionFactory    _connectionFactory;
        private readonly MessageQueueSettings  _settings;
        private readonly ILogger<HermesWorker> _logger;

        private          IConnection           _connection;

        public HermesWorker(
            IConnectionFactory             connectionFactory,
            IOptions<MessageQueueSettings> options,
            ILogger<HermesWorker>          logger) {
            _connectionFactory = connectionFactory;
            _settings          = options.Value;
            _logger            = logger;
        }

        public override async Task StartAsync(CancellationToken cancellationToken) {
            _connection = await _connectionFactory.ConnectAsync(_settings);
            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken) {
            _connection.Dispose();
            await base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            var consumer = _connection.GetConsumer<Guid, Payload>(_settings.ChannelName);

            consumer.MessageReceived += OnMessageReceived;

            await consumer.ConsumeAsync(stoppingToken);

            consumer.MessageReceived -= OnMessageReceived;
        }

        private void OnMessageReceived(IChannel channel, IMessage<Guid, Payload> message) {
            _logger.LogInformation($"message {message.Value.Data} received");
        }
    }
}
