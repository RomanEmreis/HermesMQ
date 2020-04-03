using Hermes.MessageQueue.Service.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Hermes.MessageQueue.Service.Application.Extensions {
    internal static class ServiceCollectionExtensions {
        internal static IServiceCollection AddHermesServices(this IServiceCollection services) =>
            services
                .AddSingleton<IMessageDispatcher, HermesMessageDispatcher>()
                .AddSingleton<IHermesHost, HermesHost>()
                .AddHostedService<MessageDispatchingService>();

        internal static IServiceCollection ConfigureHermesHosting(this IServiceCollection services) =>
            services.Configure<HermesHostingSettings>(settings => {
                var deploymentPortValue = Environment.GetEnvironmentVariable(EnvironmentVariables.DeploymentPort);
                if (int.TryParse(deploymentPortValue, out var deploymentPort))
                    settings.Port = deploymentPort;
            });
    }
}
