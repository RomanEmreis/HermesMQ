using FluentAssertions;
using Hermes.Abstractions;
using Hermes.Infrastructure.Extensions;
using Moq;
using System;
using Xunit;

namespace Hermes.Infrastructure.Tests {
    public class ConnectionExtensionsTests {
        private readonly Mock<IConnection> _connectionMock = new Mock<IConnection>();

        public ConnectionExtensionsTests() {
            _connectionMock
                .SetupGet(c => c.AssociatedChannel)
                .Returns(Mock.Of<IDuplexChannel>());
        }

        [Fact]
        public void GetProducer_Should_Not_Be_Null() {
            var producer = _connectionMock.Object.GetProducer<int, int>("test");
            producer.Should().NotBeNull();
        }

        [Fact]
        public void GetProducer_Null_Connection_Argument_Should_Throw_ArgumentNullException() {
            IConnection connection = default;
            Action act = () => connection.GetProducer<int, int>("test");
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("")]
        [InlineData(null)]
        public void GetProducer_Null_ChannelName_Argument_Should_Throw_ArgumentException(string channelName) {
            Action act = () => _connectionMock.Object.GetProducer<int, int>(channelName);
            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void GetConsumer_Should_Not_Be_Null() {
            var consumer = _connectionMock.Object.GetConsumer<int, int>("test");
            consumer.Should().NotBeNull();
        }

        [Fact]
        public void GetConsumer_Null_Connection_Argument_Should_Throw_ArgumentNullException() {
            IConnection connection = default;
            Action act = () => connection.GetConsumer<int, int>("test");
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("")]
        [InlineData(null)]
        public void GetConsumer_Null_ChannelName_Argument_Should_Throw_ArgumentException(string channelName) {
            Action act = () => _connectionMock.Object.GetConsumer<int, int>(channelName);
            act.Should().ThrowExactly<ArgumentException>();
        }
    }
}
