using Hermes.Abstractions;
using Hermes.Infrastructure.Messaging;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Hermes.Infrastructure.Hosting {
    public sealed class DefaultHermesMessageDispatcher : IMessageDispatcher {
        private readonly ConcurrentBag<IConnection> _connections = new ConcurrentBag<IConnection>();
        private readonly ILogger                    _logger;
        private readonly IProducer<object>          _producer;
        private readonly IConsumer<object>          _consumer;

        public DefaultHermesMessageDispatcher(IMessageAdapter messageAdapter, ILogger logger) {
            _logger   = logger;
            _consumer = new DefaultHermesConsumer<object>(messageAdapter, logger);
            _producer = new DefaultHermesProducer<object>(messageAdapter, logger);
        }

        public void AddConnection(IConnection connection) {
            var duplexChannel = connection.GetOrCreateDuplexChannel("dispatcher_internal_channel");
            _connections.Add(connection);

            _ = _consumer.ConsumeAsync(duplexChannel);
        }

        public async Task DispatchAsync() {
            _consumer.OnMessageReceived += OnMessageReceived;
        }

        private void OnMessageReceived(IChannel channel, object message) {
            _logger.LogInformation("Start broadcasting message to consumers of channel {Channel}", channel.Name);

            foreach (var connection in _connections) {
                _logger.LogInformation("Send message to connection");

                _ = _producer.ProduceAsync((IChannelWriter)channel, message);
            }
        }
    }
}
