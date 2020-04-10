using System;
using System.Threading.Tasks;

namespace Hermes.Abstractions {
    public interface IConnectionFactory : IDisposable {
        ValueTask<IConnection> ConnectAsync(string hostAddress, int port);
    }
}
