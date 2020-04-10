using Hermes.Abstractions;
using System.Threading.Tasks;

namespace HermesMQ.Extensions {
    public static class ConnectionFactoryExtensions {
        public static ValueTask<IConnection> ConnectAsync(this IConnectionFactory connectionFactory, HermesSettings hermesSettings) =>
            connectionFactory.ConnectAsync(hermesSettings.HostAddress, hermesSettings.Port);
    }
}
