using System;
using System.Threading.Tasks;

namespace Hermes.MessageQueue.Storage {
    public interface IChannelStorage : IDisposable {
        string ChannelName { get; }

        void Add(Guid messageId, in Memory<byte> messageBytes);

        void Commit(Guid messageId, Guid connectionId);

        ValueTask<DeliveryContext[]> FetchUncommitedAsync(Guid connectionId);
    }
}
