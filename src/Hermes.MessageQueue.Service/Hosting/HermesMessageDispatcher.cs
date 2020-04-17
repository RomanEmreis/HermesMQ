using Hermes.MessageQueue.Service.Application.Entities;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Hermes.MessageQueue.Service.Hosting {
    internal sealed class HermesMessageDispatcher : IMessageDispatcher {
        private readonly IConnectionContextProvider       _connectionContextProvider;
        private readonly ILogger<HermesMessageDispatcher> _logger;
        private readonly Listener<MessageContext>         _listener;

        public HermesMessageDispatcher(IConnectionContextProvider connectionContextProvider, ILogger<HermesMessageDispatcher> logger) {
            _connectionContextProvider = connectionContextProvider;
            _logger                    = logger;
            _listener                  = new Listener<MessageContext>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ValueTask DispatchAsync(MessageContext entry) => _listener.Writer.WriteAsync(entry);

        public async Task StartDispatchingAsync(CancellationToken cancellationToken = default) =>
            await _listener.StartListen(OnMessageReceived, cancellationToken);

        private void OnMessageReceived(in MessageContext messageContext) {
            _logger.LogInformation("Start broadcasting message to consumers of channel {Channel}", messageContext.ToString());

            var senderConnectionId = messageContext.SenderConnectionId;

            foreach (var connection in _connectionContextProvider.GetAllExcept(senderConnectionId)) {
                _logger.LogInformation("Send message to connection");

                _ = connection.ProduceAsync(messageContext);
            }
        }
    }
}
