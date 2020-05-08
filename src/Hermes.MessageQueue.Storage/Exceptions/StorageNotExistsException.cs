using System;
using System.Runtime.Serialization;

namespace Hermes.MessageQueue.Storage {
    public class StorageNotExistsException : Exception {
        private const string _defaultMessage = "The storage for current channel is not exists. Try to register storage before.";

        public StorageNotExistsException() : base(_defaultMessage) {}

        public StorageNotExistsException(string message) : base(message) {}

        public StorageNotExistsException(string message, Exception innerException) : base(message, innerException) {}

        protected StorageNotExistsException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }
}
