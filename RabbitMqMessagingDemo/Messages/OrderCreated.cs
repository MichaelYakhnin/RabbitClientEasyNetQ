using System;

namespace RabbitMqMessagingDemo.Messages;

/// <summary>
/// Represents an order creation event.
/// </summary>
public record OrderCreated(
    Guid Id,
    string CorrelationId,
    string CustomerId,
    List<OrderItem> Items,
    decimal TotalAmount,
    DateTime CreatedAt
);

/// <summary>
/// Represents an item in an order.
/// </summary>
public record OrderItem(
    string ProductName,
    int Quantity,
    decimal UnitPrice
);
