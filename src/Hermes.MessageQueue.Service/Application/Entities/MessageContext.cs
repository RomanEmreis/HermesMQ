using Hermes.Abstractions;

namespace Hermes.MessageQueue.Service.Application.Entities {
    internal readonly struct MessageContext {
        internal MessageContext(IChannel channel, byte[] messageBytes) {
            Channel = channel;
            MessageBytes = messageBytes;
        }

        internal readonly IChannel Channel { get; }

        internal readonly byte[] MessageBytes { get; }
    }
}
