using System.Threading.Tasks;

namespace Hermes.Abstractions {
    public interface IMessageDispatcher {
        void AddConnection(IConnection connection);

        Task DispatchAsync();
    }
}
