using System.Threading.Tasks;

namespace Hermes.Abstractions {
    public delegate void ClientConnected(IConnection hermesConnection);

    public interface IHost {
        event ClientConnected ClientConnected;

        bool IsHosted { get; }

        ValueTask StartListenAsync(string hostAddress, int port);

        ValueTask StopListenAsync();
    }
}
