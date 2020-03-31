using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hermes.Abstractions {
    public delegate void MessageReceived<T>(IChannel channel, T message);

    public interface IConsumer<T> {
        event MessageReceived<T> OnMessageReceived;

        Task ConsumeAsync(IChannelReader channelReader, CancellationToken cancellationToken = default);
    }
}
