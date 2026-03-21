using System;
using Microsoft.Extensions.Logging;
using RabbitClientEasyNetQ.Contracts;
using RabbitMqMessagingDemo.Messages;

namespace RabbitMqMessagingDemo.Handlers;

/// <summary>
/// Contains multiple handlers for OrderCreated messages demonstrating the publish/subscribe pattern.
/// </summary>
public static class OrderHandlers
{
    /// <summary>
    /// Logging handler - logs order creation events to console.
    /// This is a simple handler that demonstrates basic message consumption.
    /// </summary>
    public static async Task LogOrderCreatedAsync(OrderCreated message, CancellationToken ct = default)
    {
        // In production, this would use ILogger
        Console.WriteLine($"[LOGGING HANDLER] Order Created");
        Console.WriteLine($"  CorrelationId: {message.CorrelationId}");
        Console.WriteLine($"  OrderId: {message.Id}");
        Console.WriteLine($"  CustomerId: {message.CustomerId}");
        Console.WriteLine($"  Items Count: {message.Items.Count}");
        Console.WriteLine($"  Total Amount: {message.TotalAmount:C2}");
        Console.WriteLine($"  CreatedAt: {message.CreatedAt:O}");
        Console.WriteLine();
    }

    /// <summary>
    /// Notification handler - sends email notifications for new orders.
    /// This demonstrates a handler that performs external operations.
    /// </summary>
    public static async Task SendOrderNotificationAsync(OrderCreated message, CancellationToken ct = default)
    {
        // In production, this would send an email via SMTP or an email service
        Console.WriteLine($"[NOTIFICATION HANDLER] Sending order notification");
        Console.WriteLine($"  CorrelationId: {message.CorrelationId}");
        Console.WriteLine($"  CustomerId: {message.CustomerId}");
        Console.WriteLine($"  OrderId: {message.Id}");
        Console.WriteLine($"  Subject: New Order #{message.Id} for Customer {message.CustomerId}");
        Console.WriteLine($"  Body: Your order has been created with {message.Items.Count} items totaling {message.TotalAmount:C2}");
        Console.WriteLine();
    }

    /// <summary>
    /// Inventory processing handler - reserves inventory for new orders.
    /// This demonstrates a handler that performs business logic operations.
    /// </summary>
    public static async Task ProcessOrderInventoryAsync(OrderCreated message, CancellationToken ct = default)
    {
        // In production, this would call an inventory service
        Console.WriteLine($"[INVENTORY HANDLER] Processing order inventory");
        Console.WriteLine($"  CorrelationId: {message.CorrelationId}");
        Console.WriteLine($"  OrderId: {message.Id}");
        
        foreach (var item in message.Items)
        {
            Console.WriteLine($"    Reserving: {item.ProductName} x{item.Quantity} @ {item.UnitPrice:C2}");
            
            // Simulate inventory reservation logic
            // await _inventoryService.ReserveItem(item.ProductName, item.Quantity);
        }
        
        Console.WriteLine($"  Inventory reservation completed for order {message.Id}");
        Console.WriteLine();
    }

    /// <summary>
    /// Analytics handler - tracks order creation events for analytics.
    /// This demonstrates a handler that performs analytics operations.
    /// </summary>
    public static async Task TrackOrderAnalyticsAsync(OrderCreated message, CancellationToken ct = default)
    {
        // In production, this would call an analytics service
        Console.WriteLine($"[ANALYTICS HANDLER] Tracking order creation event");
        Console.WriteLine($"  CorrelationId: {message.CorrelationId}");
        Console.WriteLine($"  Event: OrderCreated");
        Console.WriteLine($"  CustomerId: {message.CustomerId}");
        Console.WriteLine($"  OrderValue: {message.TotalAmount:C2}");
        Console.WriteLine();
    }
}
