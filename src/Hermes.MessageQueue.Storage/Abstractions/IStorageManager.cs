using System;

namespace Hermes.MessageQueue.Storage {
    public interface IStorageManager : IDisposable {
        IChannelStorage CreateStorage(string channelName);

        IChannelStorage GetStorage(string channelName);
    }
}
