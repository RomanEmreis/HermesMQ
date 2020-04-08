using Hermes.Abstractions;
using Hermes.Infrastructure.Connection;
using Hermes.Infrastructure.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace HermesMQ.Extensions {
    public static class ServiceCollectionExtensions {
        public static IServiceCollection AddHermes<TKey, TValue>(this IServiceCollection services) =>
            services
                .AddSingleton<IConnectionFactory, HermesConnectionFactory>()
                .AddScoped<IMessageAdapter, JsonMessageAdapter>();
    }
}
