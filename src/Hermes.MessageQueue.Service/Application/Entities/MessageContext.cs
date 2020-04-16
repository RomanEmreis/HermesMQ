using Hermes.Abstractions;
using System;
using System.Runtime.CompilerServices;

namespace Hermes.MessageQueue.Service.Application.Entities {
    internal readonly struct MessageContext {
        private readonly Guid   _connectionId;
        private readonly string _channelName;
        private readonly byte[] _messageBytes;

        internal MessageContext(Guid id, string channelName, byte[] messageBytes) {
            _connectionId = id;
            _channelName  = channelName;
            _messageBytes = messageBytes;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool CanBeProduced(IConnection connection, out byte[] messageBytes) {
            if (_messageBytes.Length != 0 && connection.Id != _connectionId && _channelName == connection.AssociatedChannel.Name) {
                messageBytes = _messageBytes;
                return true;
            }

            messageBytes = default;

            return false;
        }

        public override string ToString() => _channelName;
    }
}
