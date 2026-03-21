using System;

namespace RabbitMqMessagingDemo.Messages;

/// <summary>
/// Represents a user registration event.
/// </summary>
public record UserRegistered(
    Guid Id,
    string CorrelationId,
    string UserId,
    string Email,
    DateTime RegisteredAt
);
