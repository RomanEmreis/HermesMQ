using Hermes.Abstractions;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Hermes.Infrastructure.Connection {
    public class HermesConnectionFactory : IConnectionFactory {
        private readonly ILogger                                          _logger;
        private readonly ConcurrentDictionary<(int, string), IConnection> _existingConnections =
            new ConcurrentDictionary<(int, string), IConnection>();

        public HermesConnectionFactory(ILogger logger) {
            _logger = logger;
        }

        public ValueTask<IConnection> ConnectAsync(string hostAddress, int port) =>
            _existingConnections.TryGetValue((port, hostAddress), out var existingConnection)
                ? new ValueTask<IConnection>(existingConnection)
                : ConnectImplAsync(hostAddress, port);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ValueTask<IConnection> ConnectImplAsync(string hostAddress, int port) {
            _logger.LogInformation("Connection to {Host}:{Port}", hostAddress, port);

            return IPAddress.TryParse(hostAddress, out var ipAddress)
                ? new ValueTask<IConnection>(ConnectImpl(ipAddress, hostAddress, port))
                : new ValueTask<IConnection>(ConnectToHostNameAsync(hostAddress, port));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private async Task<IConnection> ConnectToHostNameAsync(string hostName, int port) {
            var addresses = await Dns.GetHostAddressesAsync(hostName);
            if (addresses.Length == 0) {
                _logger.LogError("Invalid host {Host}:{Port}", hostName, port);

                return default;
            }

            return ConnectImpl(addresses[0], hostName, port);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IConnection ConnectImpl(IPAddress ipAddress, string hostAddress, int port) {
            var newConnection = _existingConnections.GetOrAdd(
                (port, hostAddress),
                hostData => CreateConnection(ipAddress, port));

            return newConnection;
        }

        private static IConnection CreateConnection(IPAddress ipAddress, int port) {
            var clientSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(new IPEndPoint(ipAddress, port));

            var connection = new HermesConnection(clientSocket);

            return connection;
        }

        public void Dispose() {
            foreach (var connection in _existingConnections) {
                connection.Value.Dispose();
            }
        }
    }
}
