using System.Threading.Tasks;

namespace Hermes.Abstractions {
    public interface IMessageAdapter {
        ValueTask<byte[]> AdaptAsync<T>(T message);

        ValueTask<T> AdaptAsync<T>(byte[] messageBytes);
    }
}
