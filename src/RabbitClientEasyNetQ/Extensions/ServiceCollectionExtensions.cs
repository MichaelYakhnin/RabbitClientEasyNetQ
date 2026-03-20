using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitClientEasyNetQ.Contracts;
using RabbitClientEasyNetQ.Internal;

namespace RabbitClientEasyNetQ.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMessaging(this IServiceCollection services, Action<RabbitMqOptions>? configure = null)
        {
            if (configure != null)
            {
                services.Configure(configure);
            }

            services.AddSingleton<IEasyNetQBusAdapter, EasyNetQBusAdapter>();
            services.AddSingleton<IMessageBus, MessageBus>();

            // Ensure ILoggerFactory is resolvable; if host provides logging this will be a no-op
            services.AddSingleton<Microsoft.Extensions.Logging.ILoggerFactory>(provider =>
            {
                var factory = provider.GetService<Microsoft.Extensions.Logging.ILoggerFactory>();
                return factory ?? Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance;
            });

            return services;
        }
    }
}
