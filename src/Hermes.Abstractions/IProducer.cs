using System.Threading;
using System.Threading.Tasks;

namespace Hermes.Abstractions {
    public interface IProducer<TKey, TValue> {
        Task ProduceAsync(TKey key, TValue value, CancellationToken cancellationToken = default);
    }
}
