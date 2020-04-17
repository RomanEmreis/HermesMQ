using Hermes.Abstractions;
using Hermes.MessageQueue.Service.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hermes.MessageQueue.Service.Application.Entities {
    internal sealed class ConnectionContext : IConnectionContext {
        private          bool                  _disposed;
        private readonly IConnectionDispatcher _connectionDispatcher;

        internal ConnectionContext(IConnectionDispatcher connectionDispatcher, IConnection connection) {
            _connectionDispatcher = connectionDispatcher;
            Connection            = connection;
        }

        public Guid Id => Connection.Id;

        internal IConnection Connection { get; }

        public async Task ProduceAsync(MessageContext messageContext) {
            var channel = Connection.AssociatedChannel;

            if (messageContext.CanBeProduced(channel.Name, out var messageBytes)) {
                await channel.WriteAsync(messageBytes);
            }
        }

        internal async Task ConsumeAsync(IMessageDispatcher dispatcher, CancellationToken cancellationToken) {
            var channel       = Connection.AssociatedChannel;

            while (!cancellationToken.IsCancellationRequested && Connection.IsConnected) {
                var messageBytes = await channel.ReadAsync(cancellationToken);
                var context      = new MessageContext(Connection.Id, channel.Name, messageBytes);
                
                await dispatcher.DispatchAsync(context);
            }

            await DisposeAsync();
        }

        internal async Task WaitForChannelCreated(IMessageDispatcher dispatcher, CancellationToken cancellationToken) {
            await Connection.WaitForAssociations(cancellationToken);

            _ = Task.Run(() =>
                ConsumeAsync(dispatcher, cancellationToken),
                cancellationToken);
        }

        public ValueTask DisposeAsync() {
            return _disposed 
                ? new ValueTask()
                : new ValueTask(DisposeImpl());

            async Task DisposeImpl() {
                Connection.Dispose();
                await _connectionDispatcher.DispatchAsync(Connection.Id);
            }
        }
    }
}
