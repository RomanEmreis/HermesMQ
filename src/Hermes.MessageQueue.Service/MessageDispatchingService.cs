using System.Threading;
using System.Threading.Tasks;
using Hermes.MessageQueue.Service.Application;
using Hermes.MessageQueue.Service.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Hermes.MessageQueue.Service {
    internal sealed class MessageDispatchingService : BackgroundService {
        private readonly IHermesHost                        _hermesHost;
        private readonly IMessageDispatcher                 _messageDispatcher;
        private readonly ILogger<MessageDispatchingService> _logger;
        private readonly HermesHostingSettings              _hostingSettings;

        public MessageDispatchingService(
            IHermesHost                        hermesHost,
            IMessageDispatcher                 messageDispatcher,
            IOptions<HermesHostingSettings>    options,
            ILogger<MessageDispatchingService> logger) {
            _hermesHost        = hermesHost;
            _messageDispatcher = messageDispatcher;
            _hostingSettings   = options.Value;
            _logger            = logger;
        }

        public override async Task StartAsync(CancellationToken cancellationToken) {
            _logger.LogInformation("Starting Hermes Message Queue Service");

            await _hermesHost.StartListenAsync(_hostingSettings.Port);
            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken) {
            _logger.LogInformation("Stopping Hermes Message Queue Service");

            await _hermesHost.StopListenAsync();
            await base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            _logger.LogInformation("All systems go!");
            await _messageDispatcher.StartDispatchingAsync(stoppingToken);
        }
    }
}
