using System;
using System.Collections.Generic;

namespace Hermes.Abstractions {
    public interface IMessage<TKey, TValue> {
        string ChannelName { get; set; }

        DateTime CreateDate { get; set; }

        IDictionary<string, string> Headers { get; set; }

        TKey Key { get; set; }

        TValue Value { get; set; }
    }
}
