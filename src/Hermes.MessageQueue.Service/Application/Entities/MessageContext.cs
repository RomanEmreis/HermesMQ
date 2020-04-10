namespace Hermes.MessageQueue.Service.Application.Entities {
    internal readonly struct MessageContext {
        internal MessageContext(string channelName, byte[] messageBytes) {
            ChannelName  = channelName;
            MessageBytes = messageBytes;
        }

        internal readonly string ChannelName { get; }

        internal readonly byte[] MessageBytes { get; }

        internal void Deconstruct(out string channelName, out byte[] messageBytes) =>
            (channelName, messageBytes) = (ChannelName, MessageBytes);
    }
}
