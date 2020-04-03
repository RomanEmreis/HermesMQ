using Hermes.Abstractions;
using Hermes.Infrastructure.Connection;
using Hermes.Infrastructure.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace HermesMQ.Extensions {
    public static class ServiceCollectionExtensions {
        public static IServiceCollection AddHermes<TKey, TValue>(this IServiceCollection services) =>
            services
                .AddSingleton<IConnectionFactory, DefaultHermesConnectionFactory>()
                .AddScoped<IMessageAdapter, DefaultMessageAdapter>()
                .AddScoped<IProducer<TKey, TValue>, DefaultHermesProducer<TKey, TValue>>()
                .AddScoped<IConsumer<TKey, TValue>, DefaultHermesConsumer<TKey, TValue>>();

        public static IServiceCollection AddHermesConsumer<TKey, TValue>(this IServiceCollection services) =>
            services
                .AddSingleton<IConnectionFactory, DefaultHermesConnectionFactory>()
                .AddScoped<IMessageAdapter, DefaultMessageAdapter>()
                .AddScoped<IConsumer<TKey, TValue>, DefaultHermesConsumer<TKey, TValue>>();

        public static IServiceCollection AddHermesProducer<TKey, TValue>(this IServiceCollection services) =>
            services
                .AddSingleton<IConnectionFactory, DefaultHermesConnectionFactory>()
                .AddScoped<IMessageAdapter, DefaultMessageAdapter>()
                .AddScoped<IConsumer<TKey, TValue>, DefaultHermesConsumer<TKey, TValue>>();
    }
}
