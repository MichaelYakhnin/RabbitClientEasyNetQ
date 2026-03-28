using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitClientEasyNetQ;
using RabbitClientEasyNetQ.Contracts;
using RabbitClientEasyNetQ.Extensions;
using EasyNetQ;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        var conn = Environment.GetEnvironmentVariable("RABBITMQ_CONNECTION_STRING") ?? "host=localhost;port=5672;virtualHost=/;username=guest;password=guest;requestedHeartbeat=60";
        services.AddMessaging(opts => { opts.ConnectionString = conn; });
        services.AddEasyNetQ(conn);

        services.AddLogging(builder => builder.AddConsole());
    })
    .Build();

var messageBus = host.Services.GetRequiredService<IMessageBus>();

// Subscribe handlers
await messageBus.SubscribeAsync<OrderCreated>(async (order, ct) =>
{
    Console.WriteLine($"[WORKER] OrderReceived Id={order.Id} Customer={order.CustomerId} Items={order.Items.Count} Total={order.TotalAmount:C2}");
    await Task.CompletedTask;
});

Console.WriteLine("[WORKER] Subscribed to OrderCreated. Waiting for messages...");

await host.RunAsync();
