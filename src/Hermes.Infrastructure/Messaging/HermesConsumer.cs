using Hermes.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace Hermes.Infrastructure.Messaging {
    public class HermesConsumer<TKey, TValue> : IConsumer<TKey, TValue> {
        private readonly IChannelReader  _channelReader;
        private readonly IMessageAdapter _messageAdapter;

        public HermesConsumer(
            IChannelReader  channelReader,
            IMessageAdapter messageAdapter) {
            _channelReader  = channelReader;
            _messageAdapter = messageAdapter;
        }

        public event MessageReceived<TKey, TValue> MessageReceived;

        public async Task ConsumeAsync(CancellationToken cancellationToken = default) {
            while (!cancellationToken.IsCancellationRequested) {
                var messageBytes = await _channelReader.ReadAsync(cancellationToken).ConfigureAwait(false);
                var message      = await _messageAdapter.AdaptAsync<Message<TKey, TValue>>(messageBytes).ConfigureAwait(false);

                MessageReceived?.Invoke(_channelReader, message);
            }
        }
    }
}
