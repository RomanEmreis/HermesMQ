using Hermes.Abstractions;
using Hermes.Infrastructure.Messaging;

namespace Hermes.Infrastructure.Extensions {
    public static class ConnectionExtensions {
        public static IProducer<TKey, TValue> GetProducer<TKey, TValue>(this IConnection connection, string channelName) {
            var adapter = new JsonMessageAdapter();

            connection.AssociateWith(channelName);

            return new HermesProducer<TKey, TValue>(connection.AssociatedChannel, adapter);
        }

        public static IConsumer<TKey, TValue> GetConsumer<TKey, TValue>(this IConnection connection, string channelName) {
            var adapter = new JsonMessageAdapter();

            connection.AssociateWith(channelName);

            return new HermesConsumer<TKey, TValue>(connection.AssociatedChannel, adapter);
        }
    }
}
