using System.Threading;
using System.Threading.Tasks;

namespace Hermes.Abstractions {
    public interface IProducer<T> {
        Task ProduceAsync(IChannelWriter channelWriter, T payload, CancellationToken cancellationToken = default);
    }
}
