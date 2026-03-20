using System;
using System.Threading;
using System.Threading.Tasks;
using RabbitClientEasyNetQ.Contracts;

namespace RabbitClientEasyNetQ.Internal
{
    internal interface IEasyNetQBusAdapter : IAsyncDisposable
    {
        Task PublishAsync<T>(T message, CancellationToken ct = default);

        Task SubscribeAsync<T>(Func<T, CancellationToken, Task> handler, SubscriptionOptions? options = null, CancellationToken ct = default);
    }
}
