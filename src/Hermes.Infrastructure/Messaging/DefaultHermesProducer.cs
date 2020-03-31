using Hermes.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hermes.Infrastructure.Messaging {
    public class DefaultHermesProducer<T> : IProducer<T> {
        private readonly IMessageAdapter _messageAdapter;
        private readonly ILogger         _logger;

        public DefaultHermesProducer(
            IMessageAdapter messageAdapter,
            ILogger            logger) {
            _messageAdapter = messageAdapter;
            _logger         = logger;
        }

        public async Task ProduceAsync(IChannelWriter channelWriter, T payload, CancellationToken cancellationToken = default) {
            var messageData = new Message<Guid, T>(
                channelWriter.Name,
                Guid.NewGuid(),
                payload
            );

            var messageBytes = await _messageAdapter.AdaptAsync(messageData).ConfigureAwait(false);
            await channelWriter.WriteAsync(messageBytes, cancellationToken).ConfigureAwait(false);

            _logger.LogInformation("The Message (key: {MessageKy}) has been sent to HermesMQ", messageData.Key);
        }
	}
}
