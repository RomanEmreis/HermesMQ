using System;
using System.Text;

namespace Hermes.Infrastructure.Connection.Helpers {
    readonly ref struct ConnectionIdentityConverter {
        private const    int    _connectionIdLengthPosition = 0;
        private const    int    _connectionNameLengthPosition = 1;
        private const    int    _minSize = 2;

        private readonly byte[] _connectionIdBytes;
        private readonly byte[] _connectionNameBytes;

        public ConnectionIdentityConverter(in ConnectionIdentity connectionIdentity) {
            _connectionIdBytes   = connectionIdentity.ConnectionId.ToByteArray();
            _connectionNameBytes = Encoding.ASCII.GetBytes(connectionIdentity.ConnectionName);
        }

        internal ReadOnlySpan<byte> ToByteSpan() {
            var connectionIdBytesLength                  = _connectionIdBytes.Length;
            var connectionNameBytesLength                = _connectionNameBytes.Length;
                                                         
            var identityBytes                            = new byte[connectionIdBytesLength + connectionNameBytesLength + _minSize];

            identityBytes[_connectionIdLengthPosition]   = (byte)connectionIdBytesLength;
            identityBytes[_connectionNameLengthPosition] = (byte)connectionNameBytesLength;

            _connectionIdBytes.CopyTo(identityBytes, _minSize);
            _connectionNameBytes.CopyTo(identityBytes, connectionIdBytesLength);

            return identityBytes;
        }

        internal static ConnectionIdentity FromSpan(in ReadOnlySpan<byte> bytes) {
            var connectionIdBytesLength   = (int)bytes[_connectionIdLengthPosition];
            var connectionNameBytesLength = (int)bytes[_connectionNameLengthPosition];

            var connectionIdBytes         = bytes.Slice(_minSize, connectionIdBytesLength);
            var connectionNameBytes       = bytes.Slice(connectionIdBytesLength, connectionNameBytesLength);

            var connectionId              = new Guid(connectionIdBytes);
            var connectionName            = Encoding.ASCII.GetString(connectionNameBytes);

            return new ConnectionIdentity(connectionId, connectionName);
        }
    }
}
