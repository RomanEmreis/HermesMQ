using Hermes.Abstractions;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Hermes.Infrastructure.Messaging {
    public class DefaultHermesProducer<TKey, TValue> : IProducer<TKey, TValue> {
        private readonly IMessageAdapter _messageAdapter;
        private readonly ILogger         _logger;
        private readonly IChannelWriter  _channelWriter;

        public DefaultHermesProducer(
            IChannelWriter  channelWriter,
            IMessageAdapter messageAdapter,
            ILogger         logger) {
            _channelWriter  = channelWriter;
            _messageAdapter = messageAdapter;
            _logger         = logger;
        }

        public async Task ProduceAsync(TKey key, TValue value, CancellationToken cancellationToken = default) {
            var message = new Message<TKey, TValue>(_channelWriter.Name, key, value);

            var messageBytes = await _messageAdapter.AdaptAsync(message).ConfigureAwait(false);
            await _channelWriter.WriteAsync(messageBytes, cancellationToken).ConfigureAwait(false);

            _logger.LogInformation(
                "The Message (key: {MessageKey}) has been sent to HermesMQ channel {Channel}",
                message.Key,
                message.ChannelName);
        }
	}
}
