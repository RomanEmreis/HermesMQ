using System;
using System.Threading.Tasks;
using Hermes.Abstractions;
using Hermes.Infrastructure.Extensions;
using Hermes.WebApi.Client.Application;
using Hermes.WebApi.Client.Models;
using HermesMQ.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Hermes.WebApi.Client.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class HermesController : ControllerBase {
        private readonly IConnectionFactory        _connectionFactory;
        private readonly IMessageAdapter           _messageAdapter;
        private readonly MessageQueueSettings      _settings;
        private readonly ILogger<HermesController> _logger;

        public HermesController(
            IConnectionFactory        connectionFactory,
            IMessageAdapter           messageAdapter,
            IOptions<MessageQueueSettings>  options,
            ILogger<HermesController> logger) {
            _connectionFactory     = connectionFactory;
            _messageAdapter        = messageAdapter;
            _settings              = options.Value;
            _logger                = logger;
        }

        [HttpPost("{payloadData}")]
        public async Task<IActionResult> Get(string payloadData) {
            var connection  = await _connectionFactory.ConnectAsync(_settings);
            var producer    = connection.GetProducer<Guid, Payload>(_settings.ChannelName, _messageAdapter);

            var message     = await producer.ProduceAsync(Guid.NewGuid(), new Payload { Data = payloadData });

            _logger.LogInformation($"The Message (key: {message.Key}) has been sent to HermesMQ channel {message.ChannelName}");

            return Ok(new { success = true });
        }
    }
}
