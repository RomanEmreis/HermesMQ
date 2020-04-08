using Hermes.Abstractions;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Hermes.Infrastructure.Messaging {
    public class JsonMessageAdapter : IMessageAdapter {
        private readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions { 
            WriteIndented = true, 
            IgnoreNullValues = true,
            IgnoreReadOnlyProperties = false
        };

        public async ValueTask<byte[]> AdaptAsync<T>(T message) {
            using var messageStream = new MemoryStream();

            await JsonSerializer.SerializeAsync(messageStream, message, _serializerOptions).ConfigureAwait(false);

            return messageStream.ToArray();
        }

        public ValueTask<T> AdaptAsync<T>(byte[] messageBytes) {
            using var messageStream = new MemoryStream(messageBytes);

            return JsonSerializer.DeserializeAsync<T>(messageStream, _serializerOptions);
        }
    }
}
