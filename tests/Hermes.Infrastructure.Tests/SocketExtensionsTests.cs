using FluentAssertions;
using Hermes.Infrastructure.Extensions;
using System;
using System.Net.Sockets;
using Xunit;

namespace Hermes.Infrastructure.Tests {
    public class SocketExtensionsTests {
        [Fact]
        public void IsConnected_Should_Be_Equals_To() {
            using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
            var isConnected = socket.IsConnected();

            isConnected.Should().BeTrue();
        }

        [Fact]
        public void IsConnected_Null_Socket_Parameter_Should_Throw_ArgumentNullException() {
            Socket socket = default;

            Action act = () => socket.IsConnected();

            act.Should().ThrowExactly<ArgumentNullException>();
        }
    }
}
