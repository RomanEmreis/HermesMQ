using Hermes.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace Hermes.MessageQueue.Service.Hosting {
    internal interface IMessageDispatcher : IDispatcher {
        void AddConnection(IConnection connection, CancellationToken cancellationToken = default);

        Task StartDispatchingAsync(CancellationToken cancellationToken = default);
    }
}
