using System;
using Microsoft.Extensions.Logging;
using RabbitClientEasyNetQ.Contracts;
using RabbitMqMessagingDemo.Messages;

namespace RabbitMqMessagingDemo.Handlers;

/// <summary>
/// Contains multiple handlers for UserRegistered messages demonstrating the publish/subscribe pattern.
/// </summary>
public static class UserHandlers
{
    /// <summary>
    /// Logging handler - logs user registration events to console.
    /// This is a simple handler that demonstrates basic message consumption.
    /// </summary>
    public static async Task LogUserRegisteredAsync(UserRegistered message, CancellationToken ct = default)
    {
        // In production, this would use ILogger
        Console.WriteLine($"[LOGGING HANDLER] User Registered");
        Console.WriteLine($"  CorrelationId: {message.CorrelationId}");
        Console.WriteLine($"  UserId: {message.UserId}");
        Console.WriteLine($"  Email: {message.Email}");
        Console.WriteLine($"  RegisteredAt: {message.RegisteredAt:O}");
        Console.WriteLine();
    }

    /// <summary>
    /// Analytics handler - tracks user registration events for analytics.
    /// This demonstrates a handler that performs analytics operations.
    /// </summary>
    public static async Task TrackUserRegistrationAsync(UserRegistered message, CancellationToken ct = default)
    {
        // In production, this would call an analytics service
        Console.WriteLine($"[ANALYTICS HANDLER] Tracking user registration event");
        Console.WriteLine($"  CorrelationId: {message.CorrelationId}");
        Console.WriteLine($"  Event: UserRegistered");
        Console.WriteLine($"  UserId: {message.UserId}");
        Console.WriteLine($"  Email: {message.Email}");
        Console.WriteLine();
    }

    /// <summary>
    /// Notification handler - sends welcome email to newly registered users.
    /// This demonstrates a handler that performs external operations.
    /// </summary>
    public static async Task SendWelcomeEmailAsync(UserRegistered message, CancellationToken ct = default)
    {
        // In production, this would send an email via SMTP or an email service
        Console.WriteLine($"[NOTIFICATION HANDLER] Sending welcome email");
        Console.WriteLine($"  CorrelationId: {message.CorrelationId}");
        Console.WriteLine($"  UserId: {message.UserId}");
        Console.WriteLine($"  Email: {message.Email}");
        Console.WriteLine($"  Subject: Welcome to Our Platform!");
        Console.WriteLine($"  Body: Thank you for registering. Your account has been created successfully.");
        Console.WriteLine();
    }

    /// <summary>
    /// Audit handler - records user registration in audit log.
    /// This demonstrates a handler that performs compliance operations.
    /// </summary>
    public static async Task LogUserRegistrationAuditAsync(UserRegistered message, CancellationToken ct = default)
    {
        // In production, this would write to an audit database/log
        Console.WriteLine($"[AUDIT HANDLER] Recording user registration audit");
        Console.WriteLine($"  CorrelationId: {message.CorrelationId}");
        Console.WriteLine($"  Event: UserRegistered");
        Console.WriteLine($"  UserId: {message.UserId}");
        Console.WriteLine($"  Email: {message.Email}");
        Console.WriteLine($"  Timestamp: {DateTime.UtcNow:O}");
        Console.WriteLine();
    }
}
