using System.Net.Sockets;

namespace Hermes.Infrastructure.Extensions {
    internal static class SocketExtensions {
        internal static bool IsConnected(this Socket socket) {
            try {
                return !(socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
            } catch (SocketException) { 
                return false;
            }
        }
    }
}
