using Hermes.Abstractions;
using Hermes.Infrastructure.Connection.Helpers;
using Hermes.Infrastructure.Extensions;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Hermes.Infrastructure.Connection {
    public sealed class HermesConnection : IConnection {
        private volatile int    _disposed = 0;
        private const    int    _true     = 1,
                                _false    = 0;

        private readonly Socket _socket;

        public HermesConnection(Socket socket) {
            Id      = Guid.NewGuid();
            _socket = socket;
        }

        public Guid Id { get; private set; }

        public bool IsConnected => _socket.IsConnected();

        public IDuplexChannel AssociatedChannel { get; private set; }

        public void AssociateWith(string channelName) {
            var networkStream = RegisterChannel(channelName);
            AssociatedChannel = new DuplexHermesChannel(channelName, networkStream);
        }

        private NetworkStream RegisterChannel(string channelName) {
            var registrator = new ConnectionChannelRegistrator(_socket);
            registrator.Register(Id, channelName);

            return CreateNetworkStream();
        }

        private NetworkStream CreateNetworkStream() => new NetworkStream(_socket);

        public async Task WaitForAssociations(CancellationToken cancellationToken = default) {
            var associator         = new ConnectionChannelAssociator(_socket);
            var connectionIdentity = await associator.AcceptAccociation(cancellationToken).ConfigureAwait(false);
            
            Id                     = connectionIdentity.ConnectionId;
            AssociatedChannel      = new DuplexHermesChannel(
                connectionIdentity.ConnectionName,
                CreateNetworkStream());
        }

        public void Dispose() {
            if (Interlocked.CompareExchange(ref _disposed, _true, _false) == _false) {
                AssociatedChannel.Dispose();

                _socket.Disconnect(reuseSocket: false);
                _socket.Dispose();
            }
        }
    }
}
