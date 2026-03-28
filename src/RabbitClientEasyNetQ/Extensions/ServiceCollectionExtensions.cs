using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitClientEasyNetQ.Contracts;
using RabbitClientEasyNetQ.Internal;
using EasyNetQ;

namespace RabbitClientEasyNetQ.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMessaging(this IServiceCollection services, Action<RabbitMqOptions>? configure = null)
        {
            if (configure != null)
            {
                var options = new RabbitMqOptions();
                configure(options);
                services.Configure<RabbitMqConfigs>(config =>
                {
                    config.Configs.Add(options);
                });
            }

            return AddMessagingCore(services);
        }

        public static IServiceCollection AddMessaging(this IServiceCollection services)
        {
            return AddMessagingCore(services);
        }

        private static IServiceCollection AddMessagingCore(this IServiceCollection services)
        {
            services.AddOptions<RabbitMqConfigs>();
            services.AddSingleton<IConfigureOptions<RabbitMqConfigs>, DefaultRabbitMqConfigsSetup>();

            services.AddSingleton<IBusFactory, BusFactory>();

            services.AddSingleton<BusRegistry>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<RabbitMqConfigs>>().Value;
                var factory = sp.GetRequiredService<IBusFactory>();
                var busRegistry = new BusRegistry();

                foreach (var config in options.Configs)
                {
                    var key = config.ConnectionString ?? $"{config.HostName}_{config.VirtualHost}";
                    if (!busRegistry.ContainsKey(key))
                    {
                        var connectionString = BuildConnectionString(config);
                        var bus = factory.Create(connectionString);
                        busRegistry.Add(key, bus);
                    }
                }

                return busRegistry;
            });

            services.AddKeyedSingleton<IEasyNetQBusAdapter, EasyNetQBusAdapter>(
                static (IServiceProvider sp, object? key) =>
                {
                    var registry = sp.GetRequiredService<BusRegistry>();
                    var busKey = key?.ToString() ?? "default";
                    if (!registry.TryGetValue(busKey, out var bus))
                    {
                        throw new InvalidOperationException($"Bus with key '{busKey}' not found.");
                    }
                    var logger = sp.GetRequiredService<ILogger<EasyNetQBusAdapter>>();
                    return new EasyNetQBusAdapter(bus, logger);
                });

            services.AddSingleton<IMessageBus, MessageBus>();

            return services;
        }

        private static string BuildConnectionString(RabbitMqOptions options)
        {
            if (!string.IsNullOrEmpty(options.ConnectionString))
            {
                return options.ConnectionString;
            }

            if (string.IsNullOrEmpty(options.HostName))
            {
                throw new ArgumentException("RabbitMqOptions must have either a ConnectionString or a HostName defined.");
            }

            var user = options.UserName ?? "guest";
            var password = options.Password ?? "guest";
            var host = options.HostName;
            var vhost = options.VirtualHost ?? "/";
            return $"host={host};virtualHost={vhost};username={user};password={password}";
        }

        private class DefaultRabbitMqConfigsSetup : IConfigureOptions<RabbitMqConfigs>
        {
            public void Configure(RabbitMqConfigs options)
            {
                if (options.Configs.Count == 0)
                {
                    options.Configs.Add(new RabbitMqOptions
                    {
                        HostName = "localhost",
                        UserName = "guest",
                        Password = "guest",
                        VirtualHost = "/"
                    });
                }
            }
        }
    }

    internal interface IBusFactory
    {
        IBus Create(string connectionString);
    }

    internal class BusFactory : IBusFactory
    {
        private readonly ILogger<BusFactory> _logger;
        private readonly IServiceProvider _serviceProvider;

        public BusFactory(ILogger<BusFactory> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public IBus Create(string connectionString)
        {
            var tempCollection = new ServiceCollection();
            tempCollection.AddLogging();
            tempCollection.AddEasyNetQ(connectionString);
            var provider = tempCollection.BuildServiceProvider();
            return provider.GetRequiredService<IBus>();
        }
    }

    internal class BusRegistry : Dictionary<string, IBus>
    {
    }
}
