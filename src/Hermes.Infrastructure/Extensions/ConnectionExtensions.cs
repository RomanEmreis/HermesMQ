using Hermes.Abstractions;
using Hermes.Infrastructure.Messaging;
using System;

namespace Hermes.Infrastructure.Extensions {
    public static class ConnectionExtensions {
        private const string EmptyChannelNameErrorMessage = "channelName canot be null, empty or whitespace.";

        public static IProducer<TKey, TValue> GetProducer<TKey, TValue>(this IConnection connection, string channelName) {
            if (connection is null)                     throw new ArgumentNullException(nameof(connection));
            if (string.IsNullOrWhiteSpace(channelName)) throw new ArgumentException(EmptyChannelNameErrorMessage, nameof(channelName));

            var adapter = new JsonMessageAdapter();

            connection.AssociateWith(channelName);

            return new HermesProducer<TKey, TValue>(connection.AssociatedChannel, adapter);
        }

        public static IConsumer<TKey, TValue> GetConsumer<TKey, TValue>(this IConnection connection, string channelName) {
            if (connection is null)                     throw new ArgumentNullException(nameof(connection));
            if (string.IsNullOrWhiteSpace(channelName)) throw new ArgumentException(EmptyChannelNameErrorMessage, nameof(channelName));

            var adapter = new JsonMessageAdapter();

            connection.AssociateWith(channelName);

            return new HermesConsumer<TKey, TValue>(connection.AssociatedChannel, adapter);
        }
    }
}
