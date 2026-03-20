using System;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitClientEasyNetQ.Contracts
{
    public interface IMessageBus : IAsyncDisposable
    {
        Task PublishAsync<T>(T message, CancellationToken ct = default);

        Task SubscribeAsync<T>(Func<T, Task> handler, CancellationToken ct = default);

        // Optional: advanced subscribe with options
        Task SubscribeAsync<T>(Func<T, CancellationToken, Task> handler, CancellationToken ct = default);
    }
}
