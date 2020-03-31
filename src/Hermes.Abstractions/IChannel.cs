using System;

namespace Hermes.Abstractions {
    public interface IChannel : IDisposable {
        string Name { get; }
    }
}
