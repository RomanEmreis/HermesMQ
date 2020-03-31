using System.Threading;
using System.Threading.Tasks;

namespace Hermes.Abstractions {
    public interface IChannelWriter : IChannel {
        ValueTask WriteAsync(byte[] messageBytes, CancellationToken cancellationToken = default);
    }
}
