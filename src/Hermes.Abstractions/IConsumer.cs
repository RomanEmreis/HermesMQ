using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hermes.Abstractions {
    public delegate void MessageReceived<TKey, TValue>(IChannel channel, IMessage<TKey, TValue> message);

    public interface IConsumer<TKey, TValue> {
        event MessageReceived<TKey, TValue> OnMessageReceived;

        Task ConsumeAsync(CancellationToken cancellationToken = default);
    }
}
