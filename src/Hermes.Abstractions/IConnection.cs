using System;

namespace Hermes.Abstractions {
    public interface IConnection : IDisposable {
        IDuplexChannel GetOrCreateDuplexChannel(string channelName);

        IChannelReader GetOrCreateInputChannel(string channelName);

        IChannelWriter GetOrCreateOutputChannel(string channelName);
    }
}
