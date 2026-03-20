using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitClientEasyNetQ.Contracts;
using EasyNetQ;

namespace RabbitClientEasyNetQ.Internal
{
    internal class EasyNetQBusAdapter : IEasyNetQBusAdapter
    {
        private readonly RabbitMqOptions _options;
        private readonly ILogger<EasyNetQBusAdapter> _logger;
        private readonly IServiceProvider _provider;
        private readonly Lazy<IBus> _busLazy;

        public EasyNetQBusAdapter(IServiceProvider provider, ILogger<EasyNetQBusAdapter> logger)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Resolve options from DI if available
            _options = _provider.GetService<IOptions<RabbitMqOptions>>()?.Value ?? new RabbitMqOptions();

            // Lazily resolve IBus from the application's DI container; this enables using EasyNetQ's DI integration (services.AddEasyNetQ)
            _busLazy = new Lazy<IBus>(() => _provider.GetRequiredService<IBus>(), LazyThreadSafetyMode.ExecutionAndPublication);

            _logger.LogInformation("EasyNetQBusAdapter initialized (lazy DI-resolved IBus)");
        }

        public Task PublishAsync<T>(T message, CancellationToken ct = default)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            var bus = _busLazy.Value;
            _logger.LogDebug("Publishing message of type {Type}", typeof(T).FullName);

            return bus.PubSub.PublishAsync(message, ct);
        }

        public Task SubscribeAsync<T>(Func<T, CancellationToken, Task> handler, SubscriptionOptions? options = null, CancellationToken ct = default)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            var bus = _busLazy.Value;
            var subscriptionId = options?.SubscriptionId ?? typeof(T).FullName ?? Guid.NewGuid().ToString();

            _logger.LogDebug("Subscribing to message of type {Type} with subscription id {Id}", typeof(T).FullName, subscriptionId);

            // EasyNetQ PubSub SubscribeAsync signature requires a configuration action and a cancellation token in recent versions
            return bus.PubSub.SubscribeAsync<T>(subscriptionId, async (msg, msgCt) => await handler(msg, msgCt).ConfigureAwait(false), cfg => { }, ct);
        }

        public async ValueTask DisposeAsync()
        {
            if (_busLazy.IsValueCreated)
            {
                try
                {
                    var bus = _busLazy.Value;

                    if (bus is IAsyncDisposable asyncDisp)
                    {
                        await asyncDisp.DisposeAsync().ConfigureAwait(false);
                    }
                    else if (bus is IDisposable disp)
                    {
                        disp.Dispose();
                    }

                    _logger.LogDebug("Disposed EasyNetQ bus");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Exception while disposing EasyNetQ bus");
                }
            }

            return;
        }
    }
}
