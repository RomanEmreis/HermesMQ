using Hermes.Abstractions;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Hermes.Infrastructure.Messaging {
    public class DefaultHermesConsumer<TKey, TValue> : IConsumer<TKey, TValue> {
        private readonly IChannelReader  _channelReader;
        private readonly IMessageAdapter _messageAdapter;
        private readonly ILogger         _logger;

        public DefaultHermesConsumer(
            IChannelReader  channelReader,
            IMessageAdapter messageAdapter,
            ILogger         logger) {
            _channelReader  = channelReader;
            _messageAdapter = messageAdapter;
            _logger         = logger;
        }

        public event MessageReceived<TKey, TValue> OnMessageReceived;

        public async Task ConsumeAsync(CancellationToken cancellationToken = default) {
            while (!cancellationToken.IsCancellationRequested) {
                var messageBytes = await _channelReader.ReadAsync(cancellationToken).ConfigureAwait(false);
                var message      = await _messageAdapter.AdaptAsync<Message<TKey, TValue>>(messageBytes).ConfigureAwait(false);

                _logger.LogInformation(
                    "Received the message (key: {MessageKey}) of type: {Type} for {Channel}",
                    message.Key,
                    typeof(TValue),
                    message.ChannelName);

                OnMessageReceived?.Invoke(_channelReader, message);
            }
        }
    }
}
