using Hermes.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hermes.Infrastructure.Messaging {
    public class DefaultHermesConsumer<T> : IConsumer<T> {
        private readonly IMessageAdapter _messageAdapter;
        private readonly ILogger         _logger;

        public DefaultHermesConsumer(
            IMessageAdapter messageAdapter,
            ILogger            logger) {
            _messageAdapter = messageAdapter;
            _logger         = logger;
        }

        public event MessageReceived<T> OnMessageReceived;

        public async Task ConsumeAsync(IChannelReader channelReader, CancellationToken cancellationToken = default) {
            while (!cancellationToken.IsCancellationRequested) {
                var messageBytes = await channelReader.ReadAsync(cancellationToken).ConfigureAwait(false);
                var message      = await _messageAdapter.AdaptAsync<Message<Guid, T>>(messageBytes).ConfigureAwait(false);

                _logger.LogInformation(
                    "Received the message (key: {MessageKey}) of type: {Type} for {Channel}",
                    message.Key,
                    typeof(T),
                    channelReader.Name);

                OnMessageReceived?.Invoke(channelReader, message.Value);
            }
        }
    }
}
