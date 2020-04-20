using FluentAssertions;
using Hermes.Abstractions;
using Hermes.Infrastructure.Messaging;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Hermes.Infrastructure.Tests {
    public class HermesConsumerTests {
        private readonly IConsumer<int, int>   _consumer;

        private readonly Mock<IChannelReader>  _readerMock  = new Mock<IChannelReader>();
        private readonly Mock<IMessageAdapter> _adapterMock = new Mock<IMessageAdapter>();

        public HermesConsumerTests() {
            _consumer = new HermesConsumer<int, int>(
                _readerMock.Object,
                _adapterMock.Object);
        }

        [Fact]
        public async Task ConsumeAsync_MessageSent_Should_Be_Raised() {
            var cts = new CancellationTokenSource();

            var isRaised = false;
            _consumer.MessageReceived += OnMessageReceived;

            await _consumer.ConsumeAsync(cts.Token);

            _consumer.MessageReceived -= OnMessageReceived;

            isRaised.Should().BeTrue();

            void OnMessageReceived(IChannel __, IMessage<int, int> _) {
                isRaised = true;
                cts.Cancel();
            }
        }

        [Fact]
        public async Task ConsumeAsync_MessageAdapter_AdaptAsync_Should_Be_Called_Once() {
            var cts = new CancellationTokenSource();

            _consumer.MessageReceived += OnMessageReceived;

            await _consumer.ConsumeAsync(cts.Token);

            _consumer.MessageReceived -= OnMessageReceived;

            _adapterMock.Verify(a => a.AdaptAsync<Message<int, int>>(It.IsAny<byte[]>()), Times.Once);

            void OnMessageReceived(IChannel __, IMessage<int, int> _) => cts.Cancel();
        }

        [Fact]
        public async Task ConsumeAsync_ChannelReader_ReadAsync_Should_Be_Called_Once() {
            var cts = new CancellationTokenSource();

            _consumer.MessageReceived += OnMessageReceived;

            await _consumer.ConsumeAsync(cts.Token);

            _consumer.MessageReceived -= OnMessageReceived;

            _readerMock.Verify(r => r.ReadAsync(It.IsAny<CancellationToken>()), Times.Once);

            void OnMessageReceived(IChannel __, IMessage<int, int> _) => cts.Cancel();
        }
    }
}
