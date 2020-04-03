using Hermes.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Hermes.MessageQueue.Service.Hosting {
    internal sealed class HermesMessageDispatcher : IMessageDispatcher {
        private readonly ConcurrentDictionary<Guid, ConnectionContext> _connections = new ConcurrentDictionary<Guid, ConnectionContext>();
        private readonly ILogger<HermesMessageDispatcher>              _logger;

        private readonly Channel<MessageContext>                       _messagesChannel;
        private readonly Channel<Guid>                                 _connectionsChannel;

        public HermesMessageDispatcher(ILogger<HermesMessageDispatcher> logger) {
            _logger             = logger;
            _messagesChannel    = Channel.CreateUnbounded<MessageContext>();
            _connectionsChannel = Channel.CreateUnbounded<Guid>();
        }

        public void AddConnection(IConnection connection, CancellationToken cancellationToken = default) {
            var context = new ConnectionContext(connection, _messagesChannel.Writer, _connectionsChannel.Writer);

            if (_connections.TryAdd(context.Id, context)) {
                _ = context.ConsumeAsync(cancellationToken);
            } else {
                _logger.LogError("Unable to add connection with id {ConnectionId}", context.Id);
            }
        }

        public Task StartDispatchingAsync(CancellationToken cancellationToken = default) {
            var dispatchingMessagesTask = Task.Factory.StartNew(
                () => WaitingForMessages(_messagesChannel.Reader, cancellationToken),
                cancellationToken,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default
            );

            var disconnectionTask = Task.Factory.StartNew(
                () => WaitingForDisconnect(_connectionsChannel.Reader, cancellationToken),
                cancellationToken,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default
            );

            return Task.WhenAll(dispatchingMessagesTask, disconnectionTask);
        }

        private async Task WaitingForMessages(ChannelReader<MessageContext> channelReader, CancellationToken cancellationToken) {
            while (await channelReader.WaitToReadAsync(cancellationToken)) {
                if (channelReader.TryRead(out var messageContext)) {
                    OnMessageReceived(in messageContext);
                }
            }
        }

        private async Task WaitingForDisconnect(ChannelReader<Guid> channelReader, CancellationToken cancellationToken) {
            while (await channelReader.WaitToReadAsync(cancellationToken)) {
                if (channelReader.TryRead(out var connectionId)) {
                    _ = _connections.TryRemove(connectionId, out var _);
                }
            }
        }

        private void OnMessageReceived(in MessageContext messageContext) {
            _logger.LogInformation("Start broadcasting message to consumers of channel {Channel}", messageContext.Channel.Name);

            foreach (var connection in _connections) {
                _logger.LogInformation("Send message to connection");

                _ = connection.Value.ProduceAsync(messageContext.MessageBytes);
            }
        }

        internal readonly struct ConnectionContext {
            private readonly IDuplexChannel                   _channel;
            private readonly ChannelWriter<MessageContext>    _messageChannelWriter;
            private readonly ChannelWriter<Guid>              _connectionChannelWriter;

            internal ConnectionContext(IConnection connection, ChannelWriter<MessageContext> messageChannelWriter, ChannelWriter<Guid> connectionChannelWriter) {
                Id                       = Guid.NewGuid();
                Connection               = connection;
                _channel                 = connection.GetOrCreateDuplexChannel($"dispatcher_internal_broadcasting_channel_{Id.ToString()}");
                _messageChannelWriter    = messageChannelWriter;
                _connectionChannelWriter = connectionChannelWriter;
            }

            internal readonly IConnection Connection { get; }

            internal readonly Guid Id { get; }

            internal async Task ProduceAsync(byte[] messageBytes) {
                if (messageBytes.Length != 0)
                    await _channel.WriteAsync(messageBytes).ConfigureAwait(false);
            }

            internal async Task ConsumeAsync(CancellationToken cancellationToken) {
                while (!cancellationToken.IsCancellationRequested && Connection.IsConnected) {
                    var messageBytes = await _channel.ReadAsync(cancellationToken).ConfigureAwait(false);
                    var context      = new MessageContext(_channel, messageBytes);

                    await _messageChannelWriter.WriteAsync(context).ConfigureAwait(false);
                }

                Connection.Dispose();
                await _connectionChannelWriter.WriteAsync(Id);
            }
        }

        internal readonly struct MessageContext {
            internal MessageContext(IChannel channel, byte[] messageBytes) {
                Channel = channel;
                MessageBytes = messageBytes;
            }

            internal readonly IChannel Channel { get; }

            internal readonly byte[] MessageBytes { get; }
        }
    }
}
