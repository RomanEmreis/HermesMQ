using System;

namespace Hermes.Infrastructure.Connection.Helpers {
    internal readonly struct ConnectionIdentity {
        internal readonly string ConnectionName;
        internal readonly Guid   ConnectionId;

        internal ConnectionIdentity(Guid id, string name) {
            ConnectionId   = id;
            ConnectionName = name;
        }

        internal ReadOnlySpan<byte> ToByteSpan() {
            var converter = new ConnectionIdentityConverter(in this);
            return converter.ToByteSpan();
        }

        internal static ConnectionIdentity FromSpan(in ReadOnlySpan<byte> bytes) =>
            ConnectionIdentityConverter.FromSpan(in bytes);
    }
}
