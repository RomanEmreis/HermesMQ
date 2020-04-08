using Hermes.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace Hermes.Infrastructure.Messaging {
    public class HermesProducer<TKey, TValue> : IProducer<TKey, TValue> {
        private readonly IMessageAdapter _messageAdapter;
        private readonly IChannelWriter  _channelWriter;

        public HermesProducer(
            IChannelWriter  channelWriter,
            IMessageAdapter messageAdapter) {
            _channelWriter  = channelWriter;
            _messageAdapter = messageAdapter;
        }

        public event MessageSent<TKey, TValue> MessageSent;

        public async Task ProduceAsync(TKey key, TValue value, CancellationToken cancellationToken = default) {
            var message = new Message<TKey, TValue>(_channelWriter.Name, key, value);

            var messageBytes = await _messageAdapter.AdaptAsync(message).ConfigureAwait(false);
            await _channelWriter.WriteAsync(messageBytes, cancellationToken).ConfigureAwait(false);

            MessageSent?.Invoke(message);
        }
	}
}
