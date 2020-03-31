using System;
using System.Collections.Generic;

namespace Hermes.Abstractions {
    public class Message<TKey, TValue> : IMessage<TKey, TValue> {
        public Message() {
        }

        public Message(string channelName, TKey key, TValue value) {
            CreateDate = DateTime.Now;
            ChannelName = channelName;
            Key = key;
            Value = value;
            Headers = new Dictionary<string, string>();
        }

        public string ChannelName { get; set; }

        public DateTime CreateDate { get; set; }

        public IDictionary<string, string> Headers { get; set; }

        public TKey Key { get; set; }

        public TValue Value { get; set; }
    }
}
