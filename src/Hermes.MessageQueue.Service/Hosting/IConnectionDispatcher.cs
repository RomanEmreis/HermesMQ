using Hermes.Abstractions;
using Hermes.MessageQueue.Storage;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hermes.MessageQueue.Service.Hosting {
    internal interface IConnectionDispatcher : IDispatcher<Guid> {
        IStorageManager StorageManager { get; }

        Task AddConnectionAsync(IConnection connection, CancellationToken cancellationToken = default);
    }
}
