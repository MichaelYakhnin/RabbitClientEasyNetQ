using System.Threading;
using System.Threading.Tasks;

namespace RabbitClientEasyNetQ.Middleware
{
    public interface IConsumeMiddleware
    {
        Task InvokeAsync<T>(T message, CancellationToken ct, Func<Task> next);
    }
}
