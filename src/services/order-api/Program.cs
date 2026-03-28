using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RabbitClientEasyNetQ;
using RabbitClientEasyNetQ.Contracts;
using RabbitClientEasyNetQ.Extensions;
using EasyNetQ;

var builder = WebApplication.CreateBuilder(args);

var conn = Environment.GetEnvironmentVariable("RABBITMQ_CONNECTION_STRING") ?? "host=localhost;port=5672;virtualHost=/;username=guest;password=guest;requestedHeartbeat=60";

// Register messaging and EasyNetQ
builder.Services.AddMessaging(opts => { opts.ConnectionString = conn; });
builder.Services.AddEasyNetQ(conn);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order API V1"));

app.MapPost("/orders", async (CreateOrderRequest req, IMessageBus bus) =>
{
    var items = req.Items?.Select(i => new OrderItem(i.ProductName, i.Quantity, i.UnitPrice)).ToList() ?? new List<OrderItem>();
    var total = items.Sum(i => i.Quantity * i.UnitPrice);

    var order = new OrderCreated(
        Id: Guid.NewGuid(),
        CorrelationId: Guid.NewGuid().ToString("N"),
        CustomerId: req.CustomerId,
        Items: items,
        TotalAmount: total,
        CreatedAt: DateTime.UtcNow
    );

    await bus.PublishAsync(order);

    return Results.Created($"/orders/{order.Id}", order);
});

app.MapGet("/health", () => Results.Ok("ok"));

app.Run();

record CreateOrderRequest(string CustomerId, List<CreateOrderItem> Items);
record CreateOrderItem(string ProductName, int Quantity, decimal UnitPrice);
