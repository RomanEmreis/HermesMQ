using System;
using System.Threading;

namespace Hermes.Infrastructure.Connection {
    public abstract class ChannelBase : IDisposable {
        private volatile int _disposed = 0;
        private const int    _true     = 1,
                             _false    = 0;

        private void DisposeImpl(bool disposing) {
            if (Interlocked.CompareExchange(ref _disposed, _true, _false) == _false) {
                Dispose(disposing);
                _ = Interlocked.Exchange(ref _disposed, _true);
            }
        }

        protected abstract void Dispose(bool disposing);

        public void Dispose() => DisposeImpl(true);
    }
}
