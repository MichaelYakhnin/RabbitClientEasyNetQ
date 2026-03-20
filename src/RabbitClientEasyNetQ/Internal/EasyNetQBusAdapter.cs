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

        public EasyNetQBusAdapter(IOptions<RabbitMqOptions> options, ILogger<EasyNetQBusAdapter> logger)
        {
            _options = options?.Value ?? new RabbitMqOptions();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // EasyNetQ wiring to be implemented in a follow-up change; keep adapter inert for now
            _logger.LogInformation("EasyNetQBusAdapter initialized (wiring deferred)");
        }

        public Task PublishAsync<T>(T message, CancellationToken ct = default)
        {
            _logger.LogDebug("Publish requested for {Type} but adapter wiring is not implemented", typeof(T).FullName);
            throw new NotImplementedException("EasyNetQ publish not implemented yet");
        }

        public Task SubscribeAsync<T>(Func<T, CancellationToken, Task> handler, SubscriptionOptions? options = null, CancellationToken ct = default)
        {
            _logger.LogDebug("Subscribe requested for {Type} but adapter wiring is not implemented", typeof(T).FullName);
            throw new NotImplementedException("EasyNetQ subscribe not implemented yet");
        }

        public ValueTask DisposeAsync()
        {
            // no resources yet
            return ValueTask.CompletedTask;
        }
    }
}
