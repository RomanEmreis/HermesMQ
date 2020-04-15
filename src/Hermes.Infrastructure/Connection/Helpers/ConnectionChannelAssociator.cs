using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Hermes.Infrastructure.Connection.Helpers {
    internal readonly struct ConnectionChannelAssociator {
        private readonly Socket _socket;

        internal ConnectionChannelAssociator(Socket socket) => _socket = socket;

        public async Task<ConnectionIdentity> AcceptAccociation(CancellationToken cancellationToken = default) {
            using var registrationPipe = new InputHermesChannel(string.Empty, new NetworkStream(_socket));

            var connectionIdentity = await registrationPipe.ReadAsync(cancellationToken).ConfigureAwait(false);
            return ConnectionIdentity.FromSpan(connectionIdentity);
        }
    }
}
