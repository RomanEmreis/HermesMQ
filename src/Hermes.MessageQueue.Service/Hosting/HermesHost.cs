﻿using Hermes.Abstractions;
using Hermes.Infrastructure.Connection;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Hermes.MessageQueue.Service.Hosting {
    internal sealed class HermesHost : IHermesHost {
        private const    int                     _initialBacklog = 120;
        
        private readonly ILogger<HermesHost>     _logger;
        private          IConnection             _currentConnection;
        private readonly IConnectionDispatcher      _messageDispatcher;

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public HermesHost(IConnectionDispatcher messageDispatcher, ILogger<HermesHost> logger) {
            _logger  = logger;
            _messageDispatcher = messageDispatcher;
        }

        public event ClientConnected ClientConnected;

        public bool IsHosted { get; private set; }

        public Task StartListenAsync(int port) => !IsHosted
            ? Task.Run(() => StartListenImpl(port))
            : Task.CompletedTask;

        private void StartListenImpl(int port) {
            var listenSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            var endpoint     = new IPEndPoint(IPAddress.Any, port);

            listenSocket.Bind(endpoint);
            listenSocket.Listen(_initialBacklog);

            _logger.LogInformation("Now listening on {Host}:{Port}", endpoint.Address, port);

            IsHosted = true;

            _ = Task.Factory.StartNew(
                () => AcceptConnections(listenSocket),
                _cts.Token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default
            );

            _currentConnection = new HermesConnection(listenSocket);
        }

        private async Task AcceptConnections(Socket listenSocket) {
            while (!_cts.IsCancellationRequested) {
                var newConnection = await AcceptConnection(listenSocket);
                _logger.LogInformation("New client connected");
                
                await _messageDispatcher.AddConnectionAsync(newConnection, _cts.Token);

                ClientConnected?.Invoke(newConnection);
            }
        }

        private async Task<IConnection> AcceptConnection(Socket listenSocket) {
            var newConnectionSocket = await listenSocket.AcceptAsync();
            return new HermesConnection(newConnectionSocket);
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
