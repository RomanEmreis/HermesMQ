using System.Threading;
using System.Threading.Tasks;

namespace Hermes.MessageQueue.Service.Hosting {
    internal interface IDispatcher<T> {
        ValueTask DispatchAsync(T entry);

        Task StartDispatchingAsync(CancellationToken cancellationToken = default);
    }
}
