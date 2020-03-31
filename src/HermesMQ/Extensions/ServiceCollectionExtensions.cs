using Hermes.Abstractions;
using Hermes.Infrastructure.Connection;
using Hermes.Infrastructure.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace HermesMQ.Extensions {
    public static class ServiceCollectionExtensions {
        public static IServiceCollection AddHermes<T>(this IServiceCollection services) =>
            services
                .AddSingleton<IConnectionFactory, DefaultHermesConnectionFactory>()
                .AddScoped<IMessageAdapter, DefaultMessageAdapter>()
                .AddScoped<IProducer<T>, DefaultHermesProducer<T>>()
                .AddScoped<IConsumer<T>, DefaultHermesConsumer<T>>();

        public static IServiceCollection AddHermesConsumer<T>(this IServiceCollection services) =>
            services
                .AddSingleton<IConnectionFactory, DefaultHermesConnectionFactory>()
                .AddScoped<IMessageAdapter, DefaultMessageAdapter>()
                .AddScoped<IConsumer<T>, DefaultHermesConsumer<T>>();

        public static IServiceCollection AddHermesProducer<T>(this IServiceCollection services) =>
            services
                .AddSingleton<IConnectionFactory, DefaultHermesConnectionFactory>()
                .AddScoped<IMessageAdapter, DefaultMessageAdapter>()
                .AddScoped<IConsumer<T>, DefaultHermesConsumer<T>>();
    }
}
