using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Hermes.MessageQueue.Service.Application.Entities {
    internal delegate void ActionRef<T>(in T obj);

    internal sealed class Listener<T> {
        private readonly Channel<T> _listenChannel;

        internal Listener() {
            _listenChannel = Channel.CreateUnbounded<T>();
        }

        internal ChannelReader<T> Reader => _listenChannel.Reader;

        internal ChannelWriter<T> Writer => _listenChannel.Writer;

        internal Task StartListen(ActionRef<T> waitedDelegate, CancellationToken cancellationToken = default) =>
                Task.Factory.StartNew(
                () => WaitingFor(_listenChannel.Reader, waitedDelegate, cancellationToken),
                cancellationToken,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default
            );

        private async Task WaitingFor(ChannelReader<T> channelReader, ActionRef<T> waitedDelegate, CancellationToken cancellationToken) {
            while (await channelReader.WaitToReadAsync(cancellationToken)) {
                if (channelReader.TryRead(out var waitedObject)) {
                    waitedDelegate(waitedObject);
                }
            }
        }
    }
}
