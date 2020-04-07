using Hermes.Abstractions;
using Hermes.MessageQueue.Service.Application.Entities;
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

        public HermesMessageDispatcher(ILogger<HermesMessageDispatcher> logger) {
            _logger              = logger;
            MessagesListener     = new Listener<MessageContext>();
            ConnectionsListener  = new Listener<Guid>();
        }

        public Listener<Guid> ConnectionsListener { get; }

        public Listener<MessageContext> MessagesListener { get; }

        public void AddConnection(IConnection connection, CancellationToken cancellationToken = default) {
            var context = new ConnectionContext(connection);

            if (_connections.TryAdd(context.Id, context)) {
                _ = context.ConsumeAsync(this, cancellationToken);
            } else {
                _logger.LogError("Unable to add connection with id {ConnectionId}", context.Id);
            }
        }

        public Task StartDispatchingAsync(CancellationToken cancellationToken = default) =>
            Task.WhenAll(
                MessagesListener.StartListen(WaitingForMessages, cancellationToken),
                ConnectionsListener.StartListen(WaitingForDisconnect, cancellationToken));

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
    }
}
