using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Hermes.MessageQueue.Service.Application.Entities {
    internal sealed class Listener<T> {
        private readonly Channel<T> _listenChannel;

        internal Listener() {
            _listenChannel = Channel.CreateUnbounded<T>();
        }

        internal ChannelReader<T> Reader => _listenChannel.Reader;

        internal ChannelWriter<T> Writer => _listenChannel.Writer;

        internal Task StartListen(Func<ChannelReader<T>, CancellationToken, Task> waitingDelegate, CancellationToken cancellationToken = default) =>
                Task.Factory.StartNew(
                () => waitingDelegate(_listenChannel.Reader, cancellationToken),
                cancellationToken,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default
            );
    }
}
