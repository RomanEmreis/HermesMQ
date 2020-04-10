using Hermes.Abstractions;
using Hermes.MessageQueue.Service.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hermes.MessageQueue.Service.Application.Entities {
    internal sealed class ConnectionContext {
        internal ConnectionContext(IConnection connection) {
            Id         = Guid.NewGuid();
            Connection = connection;
        }

        internal IConnection Connection { get; }

        internal Guid Id { get; }

        internal async Task WaitForChannelCreated(IDispatcher dispatcher, CancellationToken cancellationToken) {
            await Connection.WaitForAssociations(cancellationToken);

            _ = ConsumeAsync(dispatcher, cancellationToken);
        }

        internal async Task ProduceAsync(MessageContext messageContext) {
            var (channelName, messageBytes) = messageContext;
            var channel                     = Connection.AssociatedChannel;

            if (messageBytes.Length != 0 && channel.Name == channelName) {
                await channel.WriteAsync(messageBytes).ConfigureAwait(false);
            }
        }

        internal async Task ConsumeAsync(IDispatcher dispatcher, CancellationToken cancellationToken) {
            var channel       = Connection.AssociatedChannel;
            var messageWriter = dispatcher.MessagesListener.Writer;

            while (!cancellationToken.IsCancellationRequested && Connection.IsConnected) {
                var messageBytes = await channel.ReadAsync(cancellationToken).ConfigureAwait(false);
                var context = new MessageContext(channel.Name, messageBytes);

                await messageWriter.WriteAsync(context).ConfigureAwait(false);
            }

            Connection.Dispose();
            await dispatcher.ConnectionsListener.Writer.WriteAsync(Id).ConfigureAwait(false);
        }
    }
}
