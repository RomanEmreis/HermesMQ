using Hermes.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace Hermes.Infrastructure.Extensions {
    public static class ProducerExtensions {
        public static async Task<IMessage<TKey, TValue>> ProduceAsync<TKey, TValue>(
            this IProducer<TKey, TValue> producer,
            TKey                         key, 
            TValue                       value, 
            CancellationToken            cancellationToken = default) {
            var message = new Message<TKey, TValue>(key, value);

            await producer.ProduceAsync(message, cancellationToken);

            return message;
        }
    }
}
