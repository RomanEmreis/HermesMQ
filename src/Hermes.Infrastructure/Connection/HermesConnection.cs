using Hermes.Abstractions;
using Hermes.Infrastructure.Extensions;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hermes.Infrastructure.Connection {
    public sealed class HermesConnection : IConnection {
        private readonly Socket _socket;

        public HermesConnection(Socket socket) {
            _socket = socket;
        }

        public bool IsConnected => _socket.IsConnected();

        public IDuplexChannel AssociatedChannel { get; private set; }

        public void AssociateWith(string channelName) {
            var networkStream = RegisterChannel(channelName);
            AssociatedChannel = new DuplexHermesChannel(channelName, networkStream);
        }

        private NetworkStream RegisterChannel(string channelName) {
            using var registrationPipe = new OutputHermesChannel(string.Empty, CreateNetworkStream());

            var channelNameBytes = Encoding.UTF8.GetBytes(channelName);
            _ = registrationPipe.WriteAsync(channelNameBytes);

            return CreateNetworkStream();
        }

        private NetworkStream CreateNetworkStream() => new NetworkStream(_socket);

        public async Task WaitForAssociations(CancellationToken cancellationToken = default) {
            using var registrationPipe = new InputHermesChannel(string.Empty, CreateNetworkStream());

            var channelNameBytes       = await registrationPipe.ReadAsync(cancellationToken).ConfigureAwait(false);

            if (channelNameBytes.Length != 0) {
                var channelName   =  Encoding.UTF8.GetString(channelNameBytes);

                AssociatedChannel = new DuplexHermesChannel(
                    channelName,
                    CreateNetworkStream());
            }
        }

        public void Dispose() {
            AssociatedChannel.Dispose();

            _socket.Disconnect(reuseSocket: false);
            _socket.Dispose();
        }
    }
}
