using System.Threading;
using System.Threading.Tasks;

namespace Hermes.Abstractions {
    public interface IChannelReader : IChannel {
        ValueTask<byte[]> ReadAsync(CancellationToken cancellationToken = default);
    }
}
