using Hermes.Abstractions;
using Hermes.Infrastructure.Connection;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Hermes.Infrastructure.Hosting {
    public class DefaultHermesHost : IHost {
        private const    int                     _initialBacklog = 120;
        
        private readonly ILogger                 _logger;
        private          IConnection             _currentConnection;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public DefaultHermesHost(ILogger logger) {
            _logger = logger;
        }

        public event ClientConnected ClientConnected;

        public bool IsHosted { get; private set; }

        public ValueTask StartListenAsync(string hostAddress, int port) {
            if (!IsHosted) {
                if (IPAddress.TryParse(hostAddress, out var ipAddress)) {
                    var listenSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);

                    listenSocket.Bind(new IPEndPoint(ipAddress, port));
                    listenSocket.Listen(_initialBacklog);

                    _logger.LogInformation("Listening on {Host}:{Port}", hostAddress, port);

                    IsHosted = true;

                    _ = Task.Factory.StartNew(
                        () => WaitingForConnections(listenSocket),
                        _cts.Token,
                        TaskCreationOptions.LongRunning,
                        TaskScheduler.Default
                    );

                    _currentConnection = new DefaultHermesConnection(listenSocket);
                } else {
                    _logger.LogError("Invalid host address {Host}:{Port}", hostAddress, port);
                }
            }

            return new ValueTask();
        }

        private async Task WaitingForConnections(Socket listenSocket) {
            while (!_cts.IsCancellationRequested) {
                var newConnectionSocket = await listenSocket.AcceptAsync().ConfigureAwait(false);

                _logger.LogInformation("New client connected");

                ClientConnected?.Invoke(new DefaultHermesConnection(newConnectionSocket));
            }
        }

        public ValueTask StopListenAsync() {
            if (IsHosted) {
                _cts.Cancel();

                _currentConnection.Dispose();
                _currentConnection = null;

                IsHosted = false;
            }

            return new ValueTask();
        }
    }
}
