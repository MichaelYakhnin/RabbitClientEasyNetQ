using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RabbitClientEasyNetQ.Middleware
{
    internal class MiddlewarePipeline<T>
    {
        private readonly IReadOnlyList<IPublishMiddleware> _middlewares;

        public MiddlewarePipeline(IReadOnlyList<IPublishMiddleware> middlewares)
        {
            _middlewares = middlewares ?? Array.Empty<IPublishMiddleware>();
        }

        public Task ExecuteAsync(T message, Func<Task> final)
        {
            Func<Task> next = final;

            for (int i = _middlewares.Count - 1; i >= 0; i--)
            {
                var mw = _middlewares[i];
                var localNext = next;
                next = () => mw.InvokeAsync(message, localNext);
            }

            return next();
        }
    }
}
