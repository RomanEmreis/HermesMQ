using System;
using System.Collections.Generic;

namespace Hermes.MessageQueue.Service.Hosting {
    internal interface IConnectionContextProvider : IAsyncDisposable {
        IEnumerable<IConnectionContext> GetAllExcept(Guid contextId);

        void AddConnectionContext(IConnectionContext connectionContext);

        void RemoveConnectionContext(Guid contextId);
    }
}