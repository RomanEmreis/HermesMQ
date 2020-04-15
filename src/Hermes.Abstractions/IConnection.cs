using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hermes.Abstractions {
    public interface IConnection : IDisposable {
        Guid Id { get; }

        bool IsConnected { get; }

        IDuplexChannel AssociatedChannel { get; }

        void AssociateWith(string channelName);

        Task WaitForAssociations(CancellationToken cancellationToken = default);
    }
}
