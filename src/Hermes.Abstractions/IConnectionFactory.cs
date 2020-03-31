using System.Threading.Tasks;

namespace Hermes.Abstractions {
    public interface IConnectionFactory {
        ValueTask<IConnection> ConnectAsync(string hostAddress, int port);
    }
}
