using System.Threading.Tasks;

namespace RabbitClientEasyNetQ.Middleware
{
    public interface IPublishMiddleware
    {
        Task InvokeAsync<T>(T message, Func<Task> next);
    }
}
