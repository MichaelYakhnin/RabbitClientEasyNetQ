using System;

namespace RabbitMqMessagingDemo.Messages;

/// <summary>
/// Represents a payment processing event.
/// </summary>
public record PaymentProcessed(
    Guid Id,
    string CorrelationId,
    string PaymentId,
    string OrderId,
    decimal Amount,
    PaymentStatus Status,
    DateTime ProcessedAt
);

/// <summary>
/// Represents the status of a payment.
/// </summary>
public enum PaymentStatus
{
    /// <summary>Payment was successful.</summary>
    Success,
    
    /// <summary>Payment failed.</summary>
    Failed,
    
    /// <summary>Payment is pending processing.</summary>
    Pending
}
