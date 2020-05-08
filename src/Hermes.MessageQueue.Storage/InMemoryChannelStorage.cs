using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Hermes.MessageQueue.Storage {
    internal sealed class InMemoryChannelStorage : IChannelStorage {
        private const    string                                      _messageAlreadyInDeliveryStateErrorMessage 
            = "Message with current id has already in the delivery state";

        private readonly ConcurrentDictionary<Guid, DeliveryContext> _messageDeliveryContexts
            = new ConcurrentDictionary<Guid, DeliveryContext>();

        public InMemoryChannelStorage(string channelName) {
            ChannelName = channelName;
        }

        public string ChannelName { get; }

        public void Add(Guid messageId, in Memory<byte> messageBytes) {
            if (!_messageDeliveryContexts.TryAdd(messageId, new DeliveryContext(messageId, in messageBytes)))
                throw new InvalidOperationException(_messageAlreadyInDeliveryStateErrorMessage);
        }

        public void Commit(Guid messageId, Guid connectionId) {
            if (_messageDeliveryContexts.TryGetValue(messageId, out var deliveryContext))
                deliveryContext.Commit(connectionId);
        }

        public ValueTask<DeliveryContext[]> FetchUncommitedAsync(Guid connectionId) {
            var id         = connectionId;
            var uncommited = _messageDeliveryContexts
                .Select(c => c.Value)
                .Where(c => !c.IsProcessed(id));

            return new ValueTask<DeliveryContext[]>(uncommited.ToArray());
        }

        public void Dispose() {
            _messageDeliveryContexts.Clear();
        }
    }
}
