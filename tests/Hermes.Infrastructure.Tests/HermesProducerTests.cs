using FluentAssertions;
using Hermes.Abstractions;
using Hermes.Infrastructure.Messaging;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Hermes.Infrastructure.Tests {
    public class HermesProducerTests {
        private readonly IProducer<int, int>   _producer;

        private readonly Mock<IChannelWriter>  _writerMock  = new Mock<IChannelWriter>();
        private readonly Mock<IMessageAdapter> _adapterMock = new Mock<IMessageAdapter>();

        public HermesProducerTests() {
            _producer = new HermesProducer<int, int>(
                _writerMock.Object,
                _adapterMock.Object);
        }

        [Fact]
        public async Task ProduceAsync_MessageSent_Should_Be_Raised() {
            var isRaised = false;
            _producer.MessageSent += OnMessageSent;

            await _producer.ProduceAsync(0, 0);

            _producer.MessageSent -= OnMessageSent;

            isRaised.Should().BeTrue();

            void OnMessageSent(IMessage<int, int> _) => isRaised = true;
        }

        [Fact]
        public async Task ProduceAsync_MessageAdapter_AdaptAsync_Should_Be_Called_Once() {
            await _producer.ProduceAsync(0, 0);

            _adapterMock.Verify(a => a.AdaptAsync<Message<int, int>>(It.IsAny<Message<int, int>>()), Times.Once);
        }

        [Fact]
        public async Task ProduceAsync_ChannelWriter_WriteAsync_Should_Be_Called_Once() {
            await _producer.ProduceAsync(0, 0);

            _writerMock.Verify(w => w.WriteAsync(It.IsAny<byte[]>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
