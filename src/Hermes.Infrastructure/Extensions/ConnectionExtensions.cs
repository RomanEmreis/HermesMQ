using Hermes.Abstractions;
using Hermes.Infrastructure.Messaging;

namespace Hermes.Infrastructure.Extensions {
    public static class ConnectionExtensions {
        public static IProducer<TKey, TValue> GetProducer<TKey, TValue>(this IConnection connection, string channelName) {
            var channel = connection.GetOrCreateOutputChannel(channelName);
            var adapter = new JsonMessageAdapter();

            return new HermesProducer<TKey, TValue>(channel, adapter);
        }

        public static IConsumer<TKey, TValue> GetConsumer<TKey, TValue>(this IConnection connection, string channelName) {
            var channel = connection.GetOrCreateInputChannel(channelName);
            var adapter = new JsonMessageAdapter();

            return new HermesConsumer<TKey, TValue>(channel, adapter);
        }
    }
}
