using System;
using System.Collections.Generic;

namespace Hermes.MessageQueue.Storage {
    public readonly struct DeliveryContext {
        private const    string        _messageAlreadyProcessedErrorMessage = "Current message already processed by this connection";
        private readonly Memory<byte>  _messageBytes;

        private readonly HashSet<Guid> _processedConnections;

        public DeliveryContext(Guid messageId, in Memory<byte> messageBytes) {
            MessageId             = messageId;
            _messageBytes         = messageBytes;
            _processedConnections = new HashSet<Guid>();
        }

        public readonly Guid MessageId { get; }

        public void Commit(Guid connectionId) {
            if (!_processedConnections.Add(connectionId))
                throw new InvalidOperationException(_messageAlreadyProcessedErrorMessage);
        }

        public bool IsProcessed(Guid connectionId) => 
            _processedConnections.Contains(connectionId);

        public byte[] ToByteArray() => _messageBytes.ToArray(); 

        public override string ToString() => MessageId.ToString();
    }
}
