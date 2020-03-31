using Hermes.Abstractions;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Hermes.Infrastructure.Connection {
    public sealed class DuplexHermesChannel : IDuplexChannel {
        private readonly IChannelReader _inputChannel;
        private readonly IChannelWriter _outputChannel;

        public DuplexHermesChannel(string channelName, NetworkStream networkStream) {
            _inputChannel  = new InputHermesChannel($"input_{channelName}", networkStream);
            _outputChannel = new OutputHermesChannel($"output_{channelName}", networkStream);
            Name           = channelName;
        }

        public string Name { get; }

        public ValueTask<byte[]> ReadAsync(CancellationToken cancellationToken = default) => 
            _inputChannel.ReadAsync(cancellationToken);

        public ValueTask WriteAsync(byte[] messageBytes, CancellationToken cancellationToken = default) =>
            _outputChannel.WriteAsync(messageBytes, cancellationToken);

        public void Dispose() {
            _inputChannel.Dispose();
            _outputChannel.Dispose();
        }
    }
}
