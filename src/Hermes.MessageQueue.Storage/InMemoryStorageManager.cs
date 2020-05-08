using System;
using System.Collections.Concurrent;

namespace Hermes.MessageQueue.Storage {
    internal sealed class InMemoryStorageManager : IStorageManager {
        private readonly ConcurrentDictionary<string, IChannelStorage> _storage = new ConcurrentDictionary<string, IChannelStorage>();

        public IChannelStorage CreateStorage(string channelName) {
            if (string.IsNullOrWhiteSpace(channelName)) throw new ArgumentException(nameof(channelName));

            return _storage.GetOrAdd(
                channelName, 
                CreateStorageImpl);

            static IChannelStorage CreateStorageImpl(string channelName) => 
                new InMemoryChannelStorage(channelName);
        }

        public IChannelStorage GetStorage(string channelName) {
            if (string.IsNullOrWhiteSpace(channelName)) throw new ArgumentException(nameof(channelName));

            return _storage.TryGetValue(channelName, out var channelStorage)
                ? channelStorage
                : throw new StorageNotExistsException();
        }

        public void Dispose() {
            foreach (var storage in _storage) {
                storage.Value.Dispose();
            }
            _storage.Clear();
        }
    }
}
