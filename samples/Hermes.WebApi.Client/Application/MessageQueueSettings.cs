using HermesMQ;

namespace Hermes.WebApi.Client.Application {
    public sealed class MessageQueueSettings : HermesSettings {
        public string ChannelName { get; set; }
    }
}
