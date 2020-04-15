using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Hermes.Infrastructure.Connection.Helpers {
    internal readonly ref struct ConnectionChannelRegistrator {
        private readonly Socket _socket;

        internal ConnectionChannelRegistrator(Socket socket) => _socket = socket;

        internal void Register(Guid connectionId, string channelName) {
            var connectionIdentity = new ConnectionIdentity(connectionId, channelName);
            using var registrationPipe = new OutputHermesChannel(string.Empty, new NetworkStream(_socket));

            _ = registrationPipe.WriteAsync(connectionIdentity.ToByteSpan().ToArray());
        }
    }
}
