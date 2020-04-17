using Hermes.MessageQueue.Service.Application.Entities;
using System;
using System.Threading.Tasks;

namespace Hermes.MessageQueue.Service.Hosting {
    internal interface IConnectionContext : IAsyncDisposable {
        Guid Id { get; }

        Task ProduceAsync(MessageContext messageContext);
    }
}
