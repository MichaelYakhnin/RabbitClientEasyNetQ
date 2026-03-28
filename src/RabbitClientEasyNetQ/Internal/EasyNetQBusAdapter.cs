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
        private readonly ILogger<EasyNetQBusAdapter> _logger;
        private readonly IServiceProvider _provider;
        private readonly IBus _bus;

        public EasyNetQBusAdapter(IBus bus, ILogger<EasyNetQBusAdapter> logger)
        {
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _logger.LogInformation("EasyNetQBusAdapter initialized (lazy DI-resolved IBus)");
        }

        public Task PublishAsync<T>(T message, CancellationToken ct = default)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            _logger.LogDebug("Publishing message of type {Type}", typeof(T).FullName);

            return _bus.PubSub.PublishAsync(message, ct);
        }

        public Task SubscribeAsync<T>(Func<T, CancellationToken, Task> handler, SubscriptionOptions? options = null, CancellationToken ct = default)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            var subscriptionId = options?.SubscriptionId ?? typeof(T).FullName ?? Guid.NewGuid().ToString();

            _logger.LogDebug("Subscribing to message of type {Type} with subscription id {Id}", typeof(T).FullName, subscriptionId);

            // EasyNetQ PubSub SubscribeAsync signature requires a configuration action and a cancellation token in recent versions
            return _bus.PubSub.SubscribeAsync<T>(subscriptionId, async (msg, msgCt) => await handler(msg, msgCt).ConfigureAwait(false), cfg => { }, ct);
        }

        public async ValueTask DisposeAsync()
        {
            if (_bus is IAsyncDisposable asyncDisp)
            {
                await asyncDisp.DisposeAsync().ConfigureAwait(false);
            }
            else if (_bus is IDisposable disp)
            {
                disp.Dispose();
            }
            return;
        }
    }
}
