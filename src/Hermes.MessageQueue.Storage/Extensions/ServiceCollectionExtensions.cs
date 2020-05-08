using Microsoft.Extensions.DependencyInjection;

namespace Hermes.MessageQueue.Storage.Extensions {
    public static class ServiceCollectionExtensions {
        public static IServiceCollection AddInMemoryStorage(this IServiceCollection services) =>
            services.AddSingleton<IStorageManager, InMemoryStorageManager>();
    }
}
