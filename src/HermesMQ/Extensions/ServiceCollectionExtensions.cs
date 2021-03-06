﻿using Hermes.Abstractions;
using Hermes.Infrastructure.Connection;
using Hermes.Infrastructure.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HermesMQ.Extensions {
    public static class ServiceCollectionExtensions {
        public static IServiceCollection AddHermes(this IServiceCollection services, IConfiguration configuration) =>
            services
                .AddSingleton<IConnectionFactory, HermesConnectionFactory>()
                .AddScoped<IMessageAdapter, JsonMessageAdapter>()
                .Configure<HermesSettings>(configuration.GetSection(nameof(HermesSettings)));

        public static IServiceCollection AddHermes<TOptions>(this IServiceCollection services, IConfiguration configuration, string configurationSectionName = nameof(HermesSettings))
            where TOptions : HermesSettings =>
            services
                .AddSingleton<IConnectionFactory, HermesConnectionFactory>()
                .AddScoped<IMessageAdapter, JsonMessageAdapter>()
                .Configure<TOptions>(configuration.GetSection(configurationSectionName));
    }
}
