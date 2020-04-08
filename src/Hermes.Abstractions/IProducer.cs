using System.Threading;
using System.Threading.Tasks;

namespace Hermes.Abstractions {
    public delegate void MessageSent<TKey, TValue>(IMessage<TKey, TValue> message);

    public interface IProducer<TKey, TValue> {
        event MessageSent<TKey, TValue> MessageSent;

        Task ProduceAsync(TKey key, TValue value, CancellationToken cancellationToken = default);
    }
}
