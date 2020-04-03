using Hermes.Abstractions;
using System.Threading.Tasks;

namespace Hermes.MessageQueue.Service.Hosting {
    public delegate void ClientConnected(IConnection hermesConnection);

    public interface IHermesHost {
        event ClientConnected ClientConnected;

        bool IsHosted { get; }

        Task StartListenAsync(int port);

        ValueTask StopListenAsync();
    }
}
