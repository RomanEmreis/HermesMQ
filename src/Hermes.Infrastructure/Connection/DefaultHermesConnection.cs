using Hermes.Abstractions;
using Hermes.Infrastructure.Extensions;
using System;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace Hermes.Infrastructure.Connection {
    public sealed class DefaultHermesConnection : IConnection {
        private readonly Lazy<ConcurrentDictionary<string, IDuplexChannel>> _existingDuplexChannels = 
            new Lazy<ConcurrentDictionary<string, IDuplexChannel>>(() => new ConcurrentDictionary<string, IDuplexChannel>(), true);

        private readonly Lazy<ConcurrentDictionary<string, IChannelReader>> _existingInputChannels =
            new Lazy<ConcurrentDictionary<string, IChannelReader>>(() => new ConcurrentDictionary<string, IChannelReader>(), true);

        private readonly Lazy<ConcurrentDictionary<string, IChannelWriter>> _existingOutputChannels =
            new Lazy<ConcurrentDictionary<string, IChannelWriter>>(() => new ConcurrentDictionary<string, IChannelWriter>(), true);

        private readonly Socket                                       _socket;

        public DefaultHermesConnection(Socket socket) => _socket = socket;

        public bool IsConnected => _socket.IsConnected();

        public IDuplexChannel GetOrCreateDuplexChannel(string channelName) =>
            _existingDuplexChannels.Value.GetOrAdd(
                channelName,
                name => CreateChannel(name));

        private IDuplexChannel CreateChannel(string channelName) {
            var networkStream = new NetworkStream(_socket);
            return new DuplexHermesChannel(channelName, networkStream);
        }

        public IChannelReader GetOrCreateInputChannel(string channelName) =>
            _existingInputChannels.Value.GetOrAdd(
                channelName,
                name => CreateInputChannel(name));

        private IChannelReader CreateInputChannel(string channelName) {
            var networkStream = new NetworkStream(_socket);
            return new InputHermesChannel(channelName, networkStream);
        }

        public IChannelWriter GetOrCreateOutputChannel(string channelName) =>
            _existingOutputChannels.Value.GetOrAdd(
                channelName,
                name => CreateOutputChannel(name));

        private IChannelWriter CreateOutputChannel(string channelName) {
            var networkStream = new NetworkStream(_socket);
            return new OutputHermesChannel(channelName, networkStream);
        }

        public void Dispose() {
            ReleaseChannels(_existingDuplexChannels);
            ReleaseChannels(_existingInputChannels);
            ReleaseChannels(_existingOutputChannels);

            _socket.Disconnect(reuseSocket: false);
            _socket.Dispose();
        }

        private void ReleaseChannels<TChannel>(Lazy<ConcurrentDictionary<string, TChannel>> channels)
            where TChannel : IChannel {
            if (!channels.IsValueCreated) return;

            foreach (var channel in channels.Value) {
                channel.Value.Dispose();
            }

            channels.Value.Clear();
        }
    }
}
