using Hermes.Abstractions;
using Hermes.Infrastructure.Messaging;
using Hermes.MessageQueue.Service.Application.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading;
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
                MessagesListener.StartListen(OnMessageReceived, cancellationToken),
                ConnectionsListener.StartListen(OnDisconnected, cancellationToken));

        private void OnDisconnected(in Guid connectionId) {
            if (_connections.TryRemove(connectionId, out var _))
                _logger.LogInformation("Connection with id {ConnectionId} has been disconnected", connectionId);
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
