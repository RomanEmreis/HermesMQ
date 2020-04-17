using Hermes.Abstractions;
using Hermes.MessageQueue.Service.Application.Entities;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Hermes.MessageQueue.Service.Hosting {
    internal sealed class HermesConnectionDispatcher : IConnectionDispatcher {
        private readonly IConnectionContextProvider                    _connectionContextProvider;
        private readonly IMessageDispatcher                            _messageDispatcher;
        private readonly Listener<Guid>                                _listener;

        public HermesConnectionDispatcher(
            IConnectionContextProvider          connectionContextProvider,
            IMessageDispatcher                  messageDispatcher) {
            _connectionContextProvider = connectionContextProvider;
            _messageDispatcher         = messageDispatcher;
            _listener                  = new Listener<Guid>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ValueTask DispatchAsync(Guid entry) => _listener.Writer.WriteAsync(entry);

        public Task AddConnectionAsync(IConnection connection, CancellationToken cancellationToken = default) {
            return Task.Run(() =>
                AddConnectionImpl(connection, cancellationToken),
                cancellationToken);

            async Task AddConnectionImpl(IConnection connection, CancellationToken cancellationToken = default) {
                var context         = new ConnectionContext(this, connection);
                await context.WaitForChannelCreated(_messageDispatcher, cancellationToken);

                _connectionContextProvider.AddConnectionContext(context);
            }
        }

        public async Task StartDispatchingAsync(CancellationToken cancellationToken = default) =>
            await _listener.StartListen(OnDisconnected, cancellationToken);

        private void OnDisconnected(in Guid connectionId) => 
            _connectionContextProvider.RemoveConnectionContext(connectionId);
    }
}
