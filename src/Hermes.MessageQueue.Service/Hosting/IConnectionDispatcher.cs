using Hermes.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hermes.MessageQueue.Service.Hosting {
    internal interface IConnectionDispatcher : IDispatcher<Guid> {
        Task AddConnectionAsync(IConnection connection, CancellationToken cancellationToken = default);
    }
}
