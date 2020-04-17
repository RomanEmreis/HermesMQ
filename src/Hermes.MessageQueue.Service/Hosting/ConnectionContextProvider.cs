using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hermes.MessageQueue.Service.Hosting {
    internal sealed class ConnectionContextProvider : IConnectionContextProvider {
        private volatile int                                            _disposed           = 0;
        private const    int                                            _true               = 1,
                                                                        _false              = 0;

        private readonly ConcurrentDictionary<Guid, IConnectionContext> _connectionContexts = new ConcurrentDictionary<Guid, IConnectionContext>();
        private readonly ILogger<ConnectionContextProvider>             _logger;

        public ConnectionContextProvider(ILogger<ConnectionContextProvider> logger) {
            _logger = logger;
        }

        public IEnumerable<IConnectionContext> GetAllExcept(Guid contextId) {
            var id = contextId;
            return _connectionContexts
                .Where(c => c.Key != id)
                .Select(c => c.Value);
        }

        public void AddConnectionContext(IConnectionContext connectionContext) {
            var newConnectionId = connectionContext.Id;

            if (_connectionContexts.TryAdd(newConnectionId, connectionContext)) {
                _logger.LogInformation("Connection with id {ConnectionId} has started dispatching", newConnectionId);
            } else {
                _logger.LogError("Unable to add connection with id {ConnectionId}", newConnectionId);
            }
        }

        public void RemoveConnectionContext(Guid contextId) {
            if (_connectionContexts.TryRemove(contextId, out var _)) {
                _logger.LogInformation("Connection with id {ConnectionId} has been disconnected", contextId);
            } else {
                _logger.LogError("Unable to remove connection with id {ConnectionId}", contextId);
            }
        }

        public async ValueTask DisposeAsync() {
            if (Interlocked.CompareExchange(ref _disposed, _true, _false) == _false) {
                foreach (var connectionContext in _connectionContexts) {
                    await connectionContext.Value.DisposeAsync();
                }
            }
        }
    }
}
