using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitClientEasyNetQ.Contracts;

namespace RabbitClientEasyNetQ.Internal
{
    internal class EasyNetQBusAdapter : IEasyNetQBusAdapter
    {
        private readonly RabbitMqOptions _options;
        private readonly ILogger<EasyNetQBusAdapter> _logger;
        // TODO: hold internal EasyNetQ IBus instance here (kept internal to avoid exposure)

        public EasyNetQBusAdapter(IOptions<RabbitMqOptions> options, ILogger<EasyNetQBusAdapter> logger)
        {
            _options = options?.Value ?? new RabbitMqOptions();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // TODO: initialize EasyNetQ IBus using connection string or options
        }

        public Task PublishAsync<T>(T message, CancellationToken ct = default)
        {
            // TODO: call EasyNetQ IBus publish APIs
            _logger.LogDebug("Publishing message of type {Type}", typeof(T).FullName);
            return Task.CompletedTask;
        }

        public Task SubscribeAsync<T>(Func<T, CancellationToken, Task> handler, SubscriptionOptions? options = null, CancellationToken ct = default)
        {
            // TODO: wire EasyNetQ subscription and translate incoming messages to handler
            _logger.LogDebug("Subscribing to message of type {Type}", typeof(T).FullName);
            return Task.CompletedTask;
        }

        public ValueTask DisposeAsync()
        {
            // TODO: dispose internal IBus
            return ValueTask.CompletedTask;
        }
    }
}
