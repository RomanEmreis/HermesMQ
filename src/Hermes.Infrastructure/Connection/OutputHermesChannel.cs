using Hermes.Abstractions;
using System;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Hermes.Infrastructure.Connection {
    public sealed class OutputHermesChannel : IChannelWriter {
        private readonly NetworkStream _networkStream;
        private readonly PipeWriter    _writer;

        public OutputHermesChannel(string channelName, NetworkStream networkStream) {
            _networkStream = networkStream;
            _writer        = PipeWriter.Create(_networkStream);
            Name           = channelName;
        }

        public string Name { get; }

        public async ValueTask WriteAsync(byte[] messageBytes, CancellationToken cancellationToken = default) {
            var messageSize = messageBytes.Length;
            var memory = _writer.GetMemory(messageSize);

            messageBytes.CopyTo(memory);
            _writer.Advance(messageSize);

            _ = await _writer.FlushAsync(cancellationToken).ConfigureAwait(false);
        }

        public void Dispose() {
            _writer.CancelPendingFlush();
            _writer.Complete();
            _networkStream.Dispose();
        }
    }
}
