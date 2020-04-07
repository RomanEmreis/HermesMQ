using Hermes.Abstractions;
using Hermes.MessageQueue.Service.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hermes.MessageQueue.Service.Application.Entities {
    internal readonly struct ConnectionContext {
        private readonly IDuplexChannel                _channel;

        internal ConnectionContext(IConnection connection) {
            Id         = Guid.NewGuid();
            Connection = connection;
            _channel   = connection.GetOrCreateDuplexChannel($"dispatcher_internal_broadcasting_channel_{Id}");
        }

        internal readonly IConnection Connection { get; }

        internal readonly Guid Id { get; }

        internal async Task ProduceAsync(byte[] messageBytes) {
            if (messageBytes.Length != 0)
                await _channel.WriteAsync(messageBytes).ConfigureAwait(false);
        }

        internal async Task ConsumeAsync(IDispatcher dispatcher, CancellationToken cancellationToken) {
            var messageWriter = dispatcher.MessagesListener.Writer;

            while (!cancellationToken.IsCancellationRequested && Connection.IsConnected) {
                var messageBytes = await _channel.ReadAsync(cancellationToken).ConfigureAwait(false);
                var context      = new MessageContext(_channel, messageBytes);

                await messageWriter.WriteAsync(context).ConfigureAwait(false);
            }

            Connection.Dispose();
            await dispatcher.ConnectionsListener.Writer.WriteAsync(Id);
        }
    }
}
