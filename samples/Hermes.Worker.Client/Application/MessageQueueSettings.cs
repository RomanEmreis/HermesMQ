using HermesMQ;

namespace Hermes.Worker.Client.Application {
    public sealed class MessageQueueSettings : HermesSettings {
        public string ChannelName { get; set; }
    }
}
