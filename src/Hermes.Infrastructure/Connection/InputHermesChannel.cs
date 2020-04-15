using Hermes.Abstractions;
using System.Buffers;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Hermes.Infrastructure.Connection {
    public sealed class InputHermesChannel : ChannelBase, IChannelReader {
        private readonly NetworkStream _networkStream;
        private readonly PipeReader    _reader;

        public InputHermesChannel(string channelName, NetworkStream networkStream) {
            _networkStream = networkStream;
            _reader        = PipeReader.Create(_networkStream);
            Name           = channelName;
        }

        public string Name { get; }

        public async ValueTask<byte[]> ReadAsync(CancellationToken cancellationToken = default) {
            var result = await _reader.ReadAsync(cancellationToken).ConfigureAwait(false);
            var buffer = result.Buffer;
            try {
                return buffer.ToArray();
            } finally {
                _reader.AdvanceTo(buffer.End);
            }
        }

        protected override void Dispose(bool disposing) {
            if (!disposing) return;

            _reader.CancelPendingRead();
            _reader.Complete();
            _networkStream.Dispose();
        }
    }
}
