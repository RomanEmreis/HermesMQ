using Hermes.MessageQueue.Service.Application.Entities;
using System;

namespace Hermes.MessageQueue.Service.Hosting {
    internal interface IDispatcher {
        Listener<Guid> ConnectionsListener { get; }

        Listener<MessageContext> MessagesListener { get; }
    }
}
