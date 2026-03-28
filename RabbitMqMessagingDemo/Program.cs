using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitClientEasyNetQ.Contracts;
using RabbitClientEasyNetQ.Internal;
using RabbitClientEasyNetQ;
using RabbitClientEasyNetQ.Extensions;
using EasyNetQ;
using RabbitMqMessagingDemo.Handlers;

namespace RabbitMqMessagingDemo;

/// <summary>
/// Sample application demonstrating the RabbitClientEasyNetQ library.
/// This demo uses the RabbitClientEasyNetQ library and connects to a RabbitMQ broker
/// using EasyNetQ. Set `RABBITMQ_CONNECTION_STRING` or other env vars to configure.
/// </summary>
public static class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("=".PadRight(80, '='));
        Console.WriteLine("RabbitMQ Messaging Demo");
        Console.WriteLine("=".PadRight(80, '='));
        Console.WriteLine();

        // Create host builder with dependency injection
        using var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                // Configure RabbitMQ options from environment (or defaults)
                var connectionString = Environment.GetEnvironmentVariable("RABBITMQ_CONNECTION_STRING") ?? "amqp://guest:guest@localhost:5672";
                var host = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
                var vhost = Environment.GetEnvironmentVariable("RABBITMQ_VHOST") ?? "/";

                services.AddMessaging(opts =>
                {
                    if (!string.IsNullOrWhiteSpace(connectionString)) opts.ConnectionString = connectionString;
                    if (!string.IsNullOrWhiteSpace(host)) opts.HostName = host;
                    if (!string.IsNullOrWhiteSpace(vhost)) opts.VirtualHost = vhost;
                });

                // Build the EasyNetQ connection string from configured options or environment variables
                string BuildConnectionString()
                {
                    if (!string.IsNullOrWhiteSpace(connectionString)) return connectionString!;

                    var user = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? Environment.GetEnvironmentVariable("RABBITMQ_USERNAME") ?? "guest";
                    var pass = Environment.GetEnvironmentVariable("RABBITMQ_PASS") ?? Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD") ?? "guest";
                    var port = Environment.GetEnvironmentVariable("RABBITMQ_PORT") ?? "5672";
                    var hostPart = !string.IsNullOrWhiteSpace(host) ? host : "localhost";
                    var vhostPart = !string.IsNullOrWhiteSpace(vhost) ? vhost : "/";

                    // If virtual host is "/" then omit it from the path (default vhost)
                    var vhostSegment = vhostPart == "/" ? string.Empty : "/" + System.Uri.EscapeDataString(vhostPart);

                    return $"amqp://{user}:{pass}@{hostPart}:{port}{vhostSegment}";
                }

                var connToUse = BuildConnectionString();

                // Register EasyNetQ with the computed connection string so it respects RabbitMqOptions/env vars.
                services.AddEasyNetQ(connToUse);

                // Configure logging
                services.AddLogging(builder =>
                {
                    builder.SetMinimumLevel(LogLevel.Debug);
                    builder.AddConsole();
                });
            })
            .Build();

        var messageBus = host.Services.GetRequiredService<IMessageBus>();

        try
        {
            // Subscribe handlers and publish sample messages (same as previous logic)
            Console.WriteLine("Subscribing to OrderCreated messages...");
            await messageBus.SubscribeAsync<OrderCreated>(OrderHandlers.LogOrderCreatedAsync);
            await messageBus.SubscribeAsync<OrderCreated>(OrderHandlers.SendOrderNotificationAsync);
            await messageBus.SubscribeAsync<OrderCreated>(OrderHandlers.ProcessOrderInventoryAsync);
            await messageBus.SubscribeAsync<OrderCreated>(OrderHandlers.TrackOrderAnalyticsAsync);
            Console.WriteLine("  ✓ Handlers registered\n");

            Console.WriteLine("Publishing sample messages...");
            var orderCreated = new OrderCreated(
                Id: Guid.NewGuid(),
                CorrelationId: GenerateCorrelationId(),
                CustomerId: "CUST-12345",
                Items: new List<OrderItem> { new OrderItem("Laptop",1,999.99m) },
                TotalAmount: 999.99m,
                CreatedAt: DateTime.UtcNow
            );

            await messageBus.PublishAsync(orderCreated);
            Console.WriteLine($"  ✓ Published OrderCreated (Id: {orderCreated.Id}, CorrelationId: {orderCreated.CorrelationId})\n");

            Console.WriteLine("Waiting for message processing to complete...");
            await Task.Delay(500);

            Console.WriteLine("Sample completed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            await messageBus.DisposeAsync();
        }
    }

    private static string GenerateCorrelationId() => Guid.NewGuid().ToString("N");

    // Simple in-memory adapter for demo/testing without RabbitMQ
    private class InMemoryEasyNetQBusAdapter : IEasyNetQBusAdapter
    {
        private readonly ConcurrentDictionary<Type, List<Func<object, CancellationToken, Task>>> _handlers = new();

        public Task PublishAsync<T>(T message, CancellationToken ct = default)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (_handlers.TryGetValue(typeof(T), out var list))
            {
                var tasks = new List<Task>();
                foreach (var h in list)
                {
                    try
                    {
                        tasks.Add(h(message!, ct));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Handler invocation error: {ex.Message}");
                    }
                }
                return Task.WhenAll(tasks);
            }
            return Task.CompletedTask;
        }

        public Task SubscribeAsync<T>(Func<T, CancellationToken, Task> handler, SubscriptionOptions? options = null, CancellationToken ct = default)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            var list = _handlers.GetOrAdd(typeof(T), _ => new List<Func<object, CancellationToken, Task>>());
            list.Add((obj, token) => handler((T)obj, token));
            return Task.CompletedTask;
        }

        public ValueTask DisposeAsync()
        {
            _handlers.Clear();
            return ValueTask.CompletedTask;
        }
    }
}
