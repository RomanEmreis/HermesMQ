using System;
using System.Runtime.CompilerServices;

namespace Hermes.MessageQueue.Service.Application.Entities {
    internal readonly struct MessageContext {
        private readonly string _channelName;
        private readonly byte[] _messageBytes;

        internal MessageContext(Guid id, string channelName, byte[] messageBytes) {
            SenderConnectionId = id;
            _channelName       = channelName;
            _messageBytes      = messageBytes;
        }

        internal readonly Guid SenderConnectionId { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool CanBeProduced(string channelName, out byte[] messageBytes) {
            if (_messageBytes.Length != 0 && _channelName == channelName) {
                messageBytes = _messageBytes;
                return true;
            }

            messageBytes = default;

            return false;
        }

        public override string ToString() => _channelName;
    }
}
