using Hermes.Abstractions;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Hermes.Infrastructure.Connection {
    public class DefaultHermesConnectionFactory : IConnectionFactory {
        private readonly ILogger                                          _logger;
        private readonly ConcurrentDictionary<(int, string), IConnection> _existingConnections =
            new ConcurrentDictionary<(int, string), IConnection>();

        public DefaultHermesConnectionFactory(ILogger logger) {
            _logger = logger;
        }

        public ValueTask<IConnection> ConnectAsync(string hostAddress, int port) =>
            _existingConnections.TryGetValue((port, hostAddress), out var existingConnection)
                ? new ValueTask<IConnection>(existingConnection)
                : new ValueTask<IConnection>(ConnectImpl(hostAddress, port));

        private IConnection ConnectImpl(string hostAddress, int port) {
            _logger.LogInformation("Connection to {Host}:{Port}", hostAddress, port);

            if (IPAddress.TryParse(hostAddress, out var ipAddress)) {
                var newConnection = _existingConnections.GetOrAdd(
                    (port, hostAddress),
                    hostData => CreateConnection(ipAddress, port));

                return newConnection;
            }

            _logger.LogError("Invalid host address {Host}:{Port}", hostAddress, port);

            return null;
        }

        private static IConnection CreateConnection(IPAddress ipAddress, int port) {
            var clientSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(new IPEndPoint(ipAddress, port));

            var connection = new DefaultHermesConnection(clientSocket);

            return connection;
        }
    }
}
