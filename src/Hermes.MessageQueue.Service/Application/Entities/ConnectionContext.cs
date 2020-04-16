using Hermes.Abstractions;
using Hermes.MessageQueue.Service.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hermes.MessageQueue.Service.Application.Entities {
    internal sealed class ConnectionContext {
        internal ConnectionContext(IConnection connection) {
            Connection = connection;
            Id         = Guid.NewGuid();
        }

        internal IConnection Connection { get; }

        internal Guid Id { get; }

        internal async Task WaitForChannelCreated(IDispatcher dispatcher, CancellationToken cancellationToken) {
            await Connection.WaitForAssociations(cancellationToken);

            _ = Task.Run(() => 
                ConsumeAsync(dispatcher, cancellationToken),
                cancellationToken);
        }

        internal async Task ProduceAsync(MessageContext messageContext) {
            if (messageContext.CanBeProduced(Connection, out var messageBytes)) {
                await Connection.AssociatedChannel.WriteAsync(messageBytes);
            }
        }

        internal async Task ConsumeAsync(IDispatcher dispatcher, CancellationToken cancellationToken) {
            var channel       = Connection.AssociatedChannel;
            var messageWriter = dispatcher.MessagesListener.Writer;

            while (!cancellationToken.IsCancellationRequested && Connection.IsConnected) {
                var messageBytes = await channel.ReadAsync(cancellationToken);
                var context      = new MessageContext(Connection.Id, channel.Name, messageBytes);
                
                await messageWriter.WriteAsync(context);
            }

            Connection.Dispose();
            await dispatcher.ConnectionsListener.Writer.WriteAsync(Id);
        }
    }
}
