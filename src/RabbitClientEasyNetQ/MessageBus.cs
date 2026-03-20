using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitClientEasyNetQ.Contracts;
using RabbitClientEasyNetQ.Internal;

namespace RabbitClientEasyNetQ
{
    public class MessageBus : IMessageBus
    {
        private readonly IEasyNetQBusAdapter _adapter;
        private readonly ILogger<MessageBus> _logger;

        public MessageBus(IEasyNetQBusAdapter adapter, ILogger<MessageBus> logger)
        {
            _adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task PublishAsync<T>(T message, CancellationToken ct = default)
        {
            // Pass-through to adapter; middleware pipeline would be applied here in full implementation
            return _adapter.PublishAsync(message, ct);
        }

        public Task SubscribeAsync<T>(Func<T, Task> handler, CancellationToken ct = default)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            // Wrap simple handler to adapter signature
            return _adapter.SubscribeAsync<T>((msg, token) => handler(msg), null, ct);
        }

        public Task SubscribeAsync<T>(Func<T, CancellationToken, Task> handler, CancellationToken ct = default)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            return _adapter.SubscribeAsync<T>(handler, null, ct);
        }

        public async ValueTask DisposeAsync()
        {
            await _adapter.DisposeAsync().ConfigureAwait(false);
        }
    }
}
