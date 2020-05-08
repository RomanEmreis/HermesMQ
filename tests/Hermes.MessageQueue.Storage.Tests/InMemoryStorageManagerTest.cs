using FluentAssertions;
using System;
using Xunit;

namespace Hermes.MessageQueue.Storage.Tests {
    public class InMemoryStorageManagerTest {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void CreateStorage_Null_Or_Empty_Or_Whitespace_ChannelName_Parameter_Should_Throw_ArgumentException(string channelName) {
            var manager = new InMemoryStorageManager();

            Action act = () => manager.CreateStorage(channelName);

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Theory]
        [InlineData("test channel 1")]
        [InlineData("test_channel_2")]
        [InlineData("TestChannel3")]
        public void CreateStorage_Storage_Should_Not_Be_Null(string channelName) {
            var manager = new InMemoryStorageManager();

            var storage = manager.CreateStorage(channelName);

            storage.Should().NotBeNull();
        }

        [Theory]
        [InlineData("test channel 1")]
        [InlineData("test_channel_2")]
        [InlineData("TestChannel3")]
        public void CreateStorage_Storage_ChannelName_Should_Be_Equals_To(string channelName) {
            var manager = new InMemoryStorageManager();

            var storage = manager.CreateStorage(channelName);

            storage.ChannelName.Should().Be(channelName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void GetStorage_Null_Or_Empty_Or_Whitespace_ChannelName_Parameter_Should_Throw_ArgumentException(string channelName) {
            var manager = new InMemoryStorageManager();

            Action act = () => manager.GetStorage(channelName);

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Theory]
        [InlineData("test channel 1")]
        [InlineData("test_channel_2")]
        [InlineData("TestChannel3")]
        public void GetStorage_ExistedStorage_Should_Not_Be_Null(string channelName) {
            var manager = new InMemoryStorageManager();

            var newStorage = manager.CreateStorage(channelName);

            var existedStorage = manager.GetStorage(channelName);

            existedStorage.Should().NotBeNull();
        }

        [Theory]
        [InlineData("test channel 1")]
        [InlineData("test_channel_2")]
        [InlineData("TestChannel3")]
        public void GetStorage_ExistedStorage_ChannelName_Should_Be_Equals_To(string channelName) {
            var manager = new InMemoryStorageManager();

            var newStorage = manager.CreateStorage(channelName);

            var existedStorage = manager.GetStorage(channelName);

            existedStorage.ChannelName.Should().Be(channelName);
        }


        [Theory]
        [InlineData("test channel 1")]
        [InlineData("test_channel_2")]
        [InlineData("TestChannel3")]
        public void GetStorage_NewStorage_Instance_Should_Be_ReferenceEquals_To_ExistedStorage_With_Same_ChannelName(string channelName) {
            var manager = new InMemoryStorageManager();

            var newStorage = manager.CreateStorage(channelName);

            var existedStorage = manager.GetStorage(channelName);

            existedStorage.Should().BeSameAs(newStorage);
        }

        [Theory]
        [InlineData("test channel 1")]
        [InlineData("test_channel_2")]
        [InlineData("TestChannel3")]
        public void GetStorage_Not_Existing_Should_Throw_StorageNotExistsException(string channelName) {
            var manager = new InMemoryStorageManager();

            Action act = () => manager.GetStorage(channelName);

            act.Should().ThrowExactly<StorageNotExistsException>();
        }
    }
}
