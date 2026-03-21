using System;
using Microsoft.Extensions.Logging;
using RabbitClientEasyNetQ.Contracts;
using RabbitMqMessagingDemo.Messages;

namespace RabbitMqMessagingDemo.Handlers;

/// <summary>
/// Contains multiple handlers for PaymentProcessed messages demonstrating the publish/subscribe pattern.
/// </summary>
public static class PaymentHandlers
{
    /// <summary>
    /// Logging handler - logs payment processing events to console.
    /// This is a simple handler that demonstrates basic message consumption.
    /// </summary>
    public static async Task LogPaymentProcessedAsync(PaymentProcessed message, CancellationToken ct = default)
    {
        // In production, this would use ILogger
        Console.WriteLine($"[LOGGING HANDLER] Payment Processed");
        Console.WriteLine($"  CorrelationId: {message.CorrelationId}");
        Console.WriteLine($"  PaymentId: {message.PaymentId}");
        Console.WriteLine($"  OrderId: {message.OrderId}");
        Console.WriteLine($"  Amount: {message.Amount:C2}");
        Console.WriteLine($"  Status: {message.Status}");
        Console.WriteLine($"  ProcessedAt: {message.ProcessedAt:O}");
        Console.WriteLine();
    }

    /// <summary>
    /// Order status update handler - updates order status based on payment result.
    /// This demonstrates a handler that performs business logic operations.
    /// </summary>
    public static async Task UpdateOrderStatusAsync(PaymentProcessed message, CancellationToken ct = default)
    {
        // In production, this would call an order service
        Console.WriteLine($"[ORDER HANDLER] Updating order status");
        Console.WriteLine($"  CorrelationId: {message.CorrelationId}");
        Console.WriteLine($"  OrderId: {message.OrderId}");
        
        string newStatus = message.Status switch
        {
            PaymentStatus.Success => "Paid",
            PaymentStatus.Failed => "PaymentFailed",
            PaymentStatus.Pending => "PendingPayment",
            _ => throw new InvalidOperationException($"Unknown payment status: {message.Status}")
        };
        
        Console.WriteLine($"  New Status: {newStatus}");
        Console.WriteLine($"  Amount: {message.Amount:C2}");
        Console.WriteLine();
    }

    /// <summary>
    /// Notification handler - sends receipt email for successful payments.
    /// This demonstrates a handler that performs external operations.
    /// </summary>
    public static async Task SendPaymentReceiptAsync(PaymentProcessed message, CancellationToken ct = default)
    {
        // In production, this would send an email via SMTP or an email service
        Console.WriteLine($"[NOTIFICATION HANDLER] Sending payment receipt");
        Console.WriteLine($"  CorrelationId: {message.CorrelationId}");
        Console.WriteLine($"  OrderId: {message.OrderId}");
        
        if (message.Status == PaymentStatus.Success)
        {
            Console.WriteLine($"  Status: Success");
            Console.WriteLine($"  Amount: {message.Amount:C2}");
            Console.WriteLine($"  Subject: Payment Receipt - Order #{message.OrderId}");
            Console.WriteLine($"  Body: Your payment of {message.Amount:C2} has been successfully processed.");
        }
        else if (message.Status == PaymentStatus.Failed)
        {
            Console.WriteLine($"  Status: Failed");
            Console.WriteLine($"  Subject: Payment Failed - Order #{message.OrderId}");
            Console.WriteLine($"  Body: Your payment attempt failed. Please try again or contact support.");
        }
        else if (message.Status == PaymentStatus.Pending)
        {
            Console.WriteLine($"  Status: Pending");
            Console.WriteLine($"  Subject: Payment Pending - Order #{message.OrderId}");
            Console.WriteLine($"  Body: Your payment is being processed. You will receive a confirmation shortly.");
        }
        
        Console.WriteLine();
    }

    /// <summary>
    /// Analytics handler - tracks payment events for analytics and reporting.
    /// This demonstrates a handler that performs analytics operations.
    /// </summary>
    public static async Task TrackPaymentAnalyticsAsync(PaymentProcessed message, CancellationToken ct = default)
    {
        // In production, this would call an analytics service
        Console.WriteLine($"[ANALYTICS HANDLER] Tracking payment event");
        Console.WriteLine($"  CorrelationId: {message.CorrelationId}");
        Console.WriteLine($"  Event: PaymentProcessed");
        Console.WriteLine($"  OrderId: {message.OrderId}");
        Console.WriteLine($"  Amount: {message.Amount:C2}");
        Console.WriteLine($"  Status: {message.Status}");
        Console.WriteLine();
    }

    /// <summary>
    /// Audit handler - records payment events in audit log for compliance.
    /// This demonstrates a handler that performs compliance operations.
    /// </summary>
    public static async Task LogPaymentAuditAsync(PaymentProcessed message, CancellationToken ct = default)
    {
        // In production, this would write to an audit database/log
        Console.WriteLine($"[AUDIT HANDLER] Recording payment audit");
        Console.WriteLine($"  CorrelationId: {message.CorrelationId}");
        Console.WriteLine($"  Event: PaymentProcessed");
        Console.WriteLine($"  PaymentId: {message.PaymentId}");
        Console.WriteLine($"  OrderId: {message.OrderId}");
        Console.WriteLine($"  Amount: {message.Amount:C2}");
        Console.WriteLine($"  Status: {message.Status}");
        Console.WriteLine($"  Timestamp: {DateTime.UtcNow:O}");
        Console.WriteLine();
    }
}
