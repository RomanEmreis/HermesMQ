using Hermes.MessageQueue.Service.Application.Entities;

namespace Hermes.MessageQueue.Service.Hosting {
    internal interface IMessageDispatcher : IDispatcher<MessageContext> {
    }
}
