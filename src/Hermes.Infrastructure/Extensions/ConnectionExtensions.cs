using Hermes.Abstractions;
using Hermes.Infrastructure.Messaging;
using System;

namespace Hermes.Infrastructure.Extensions {
    public static class ConnectionExtensions {
        private const string EmptyChannelNameErrorMessage = "channelName canot be null, empty or whitespace.";

        public static IProducer<TKey, TValue> GetProducer<TKey, TValue>(this IConnection connection, string channelName, IMessageAdapter messageAdapter = default) {
            if (connection is null)                     throw new ArgumentNullException(nameof(connection));
            if (string.IsNullOrWhiteSpace(channelName)) throw new ArgumentException(EmptyChannelNameErrorMessage, nameof(channelName));

            if (!connection.IsAssociated(channelName))
                connection.AssociateWith(channelName);

            return new HermesProducer<TKey, TValue>(
                connection.AssociatedChannel, 
                messageAdapter ?? new JsonMessageAdapter());
        }

        public static IConsumer<TKey, TValue> GetConsumer<TKey, TValue>(this IConnection connection, string channelName, IMessageAdapter messageAdapter = default) {
            if (connection is null)                     throw new ArgumentNullException(nameof(connection));
            if (string.IsNullOrWhiteSpace(channelName)) throw new ArgumentException(EmptyChannelNameErrorMessage, nameof(channelName));

            if (!connection.IsAssociated(channelName))
                connection.AssociateWith(channelName);

            return new HermesConsumer<TKey, TValue>(
                connection.AssociatedChannel, 
                messageAdapter ?? new JsonMessageAdapter());
        }

        public static bool IsAssociated(this IConnection connection, string channelName) =>
            connection
                ?.AssociatedChannel
                ?.Name
                ?.Equals(channelName, StringComparison.Ordinal)
            ?? false;
    }
}
