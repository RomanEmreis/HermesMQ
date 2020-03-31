using Hermes.Abstractions;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace Hermes.Infrastructure.Connection {
    public sealed class DefaultHermesConnection : IConnection {
        private readonly ConcurrentDictionary<string, IChannel> _existingChannels = 
            new ConcurrentDictionary<string, IChannel>();

        private readonly Socket                                 _socket;

        public DefaultHermesConnection(Socket socket) => _socket = socket;

        public IDuplexChannel GetOrCreateDuplexChannel(string channelName) =>
            (IDuplexChannel)_existingChannels.GetOrAdd(
                channelName,
                name => CreateChannel(name));

        private IChannel CreateChannel(string channelName) {
            var networkStream = new NetworkStream(_socket);
            return new DuplexHermesChannel(channelName, networkStream);
        }

        public IChannelReader GetOrCreateInputChannel(string channelName) =>
            (IChannelReader) _existingChannels.GetOrAdd(
                channelName,
                name => CreateInputChannel(name));

        private IChannelReader CreateInputChannel(string channelName) {
            var networkStream = new NetworkStream(_socket);
            return new InputHermesChannel(channelName, networkStream);
        }

        public IChannelWriter GetOrCreateOutputChannel(string channelName) =>
            (IChannelWriter) _existingChannels.GetOrAdd(
                channelName,
                name => CreateOutputChannel(name));

        private IChannelWriter CreateOutputChannel(string channelName) {
            var networkStream = new NetworkStream(_socket);
            return new OutputHermesChannel(channelName, networkStream);
        }

        public void Dispose() {
            foreach (var channel in _existingChannels) {
                channel.Value.Dispose();
            }
            _existingChannels.Clear();
            _socket.Disconnect(reuseSocket: true);
        }
    }
}
