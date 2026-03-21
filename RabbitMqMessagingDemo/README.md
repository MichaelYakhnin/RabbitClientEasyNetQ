# RabbitMQ Messaging Demo

A sample console application demonstrating the **RabbitClientEasyNetQ** library with:

- Environment variable-based RabbitMQ connection configuration
- Multiple subscribers per message type (publish/subscribe pattern)
- Correlation ID tracking for end-to-end observability
- EasyNetQ DI integration with `IBus` resolution

## Table of Contents

- [Overview](#overview)
- [Prerequisites](#prerequisites)
- [Project Structure](#project-structure)
- [Configuration](#configuration)
- [Quick Start](#quick-start)
- [Architecture](#architecture)
- [Message Types](#message-types)
- [Handler Patterns](#handler-patterns)
- [Usage Examples](#usage-examples)
- [Troubleshooting](#troubleshooting)

## Overview

This sample application showcases the core features of RabbitClientEasyNetQ:

1. **Publish/Subscribe Pattern**: Multiple handlers can subscribe to the same message type, each performing different actions (logging, notifications, processing, analytics, auditing).

2. **Environment Variable Configuration**: Production-ready connection configuration using environment variables instead of hardcoded values.

3. **Correlation ID Tracking**: Every message includes a `CorrelationId` that flows through all handlers, enabling end-to-end observability.

4. **EasyNetQ DI Integration**: Leverages EasyNetQ's built-in dependency injection support with `IBus` resolution.

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- RabbitMQ broker (local or cloud)
- Environment variables configured (see [Configuration](#configuration))

### Installing RabbitMQ Locally

If you don't have RabbitMQ installed, you can run it locally:

```bash
# Windows
choco install rabbitmq

# macOS
brew install rabbitmq

# Linux
sudo apt-get install rabbitmq-server
```

## Project Structure

```
RabbitMqMessagingDemo/
├── RabbitMqMessagingDemo.csproj          # .NET 9 project file
├── Program.cs                            # Entry point with DI and EasyNetQ config
├── Messages/
│   ├── OrderCreated.cs                   # Order creation event
│   ├── UserRegistered.cs                 # User registration event
│   └── PaymentProcessed.cs               # Payment processing event
├── Handlers/
│   ├── OrderHandlers.cs                  # Multiple handlers for OrderCreated
│   ├── UserHandlers.cs                   # Multiple handlers for UserRegistered
│   └── PaymentHandlers.cs                # Multiple handlers for PaymentProcessed
└── README.md                             # This file
```

## Configuration

The application uses environment variables for RabbitMQ connection configuration:

| Variable | Description | Example Value | Required |
|----------|-------------|---------------|----------|
| `RABBITMQ_CONNECTION_STRING` | RabbitMQ AMQP connection string | `amqp://guest:guest@localhost:5672` | Yes |
| `RABBITMQ_HOST` | RabbitMQ host (optional) | `localhost` | No |
| `RABBITMQ_PORT` | RabbitMQ port (optional) | `5672` | No |
| `RABBITMQ_VHOST` | RabbitMQ virtual host (optional) | `/` | No |

### Setting Environment Variables

**Windows (PowerShell):**
```powershell
# Set for current PowerShell session
$Env:RABBITMQ_CONNECTION_STRING = "amqp://guest:guest@localhost:5672"
$Env:RABBITMQ_HOST = "localhost"
$Env:RABBITMQ_VHOST = "/"
dotnet run
```

To set the variables persistently (available in new shells), run:
```powershell
setx RABBITMQ_CONNECTION_STRING "amqp://guest:guest@localhost:5672"
setx RABBITMQ_HOST "localhost"
setx RABBITMQ_VHOST "/"
# Restart your terminal to pick up `setx` changes
```

**Windows (CMD):**
```cmd
:: Set for current CMD session
set RABBITMQ_CONNECTION_STRING=amqp://guest:guest@localhost:5672
set RABBITMQ_HOST=localhost
set RABBITMQ_VHOST=/
dotnet run
```

To set persistently for future CMD sessions:
```cmd
setx RABBITMQ_CONNECTION_STRING "amqp://guest:guest@localhost:5672"
setx RABBITMQ_HOST "localhost"
setx RABBITMQ_VHOST "/"
:: Open a new CMD window to use the new variables
```

**Linux/macOS:**
```bash
export RABBITMQ_CONNECTION_STRING="amqp://guest:guest@localhost:5672"
export RABBITMQ_HOST=localhost
export RABBITMQ_VHOST="/"
dotnet run
```

### Using Docker with RabbitMQ

To run with a local RabbitMQ container:

```bash
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 \
  -e RABBITMQ_DEFAULT_USER=guest \
  -e RABBITMQ_DEFAULT_PASS=guest \
  rabbitmq:3-management

# Then set environment variables
export RABBITMQ_CONNECTION_STRING="amqp://guest:guest@localhost:5672"
dotnet run
```

## Quick Start

### Build the Project

```bash
dotnet build
```

### Run the Application

```bash
dotnet run
```

### Expected Output

```
================================================================================
RabbitMQ Messaging Demo - RabbitClientEasyNetQ Sample Application
================================================================================

Configuration:
  RabbitMQ Host: localhost
  RabbitMQ Virtual Host: /
  Connection String: amqp://guest...672

Subscribing to OrderCreated messages...
  ✓ Logging handler registered
  ✓ Notification handler registered
  ✓ Inventory processing handler registered
  ✓ Analytics handler registered

Subscribing to UserRegistered messages...
  ✓ Logging handler registered
  ✓ Analytics handler registered
  ✓ Welcome email handler registered
  ✓ Audit handler registered

Subscribing to PaymentProcessed messages...
  ✓ Logging handler registered
  ✓ Order status update handler registered
  ✓ Receipt notification handler registered
  ✓ Analytics handler registered
  ✓ Audit handler registered

Publishing sample messages...
--------------------------------------------------------------------------------
  ✓ Published OrderCreated (Id: ..., CorrelationId: ...)

  ✓ Published UserRegistered (Id: ..., CorrelationId: ...)

  ✓ Published PaymentProcessed (Id: ..., CorrelationId: ..., Status: Success)

  ✓ Published PaymentProcessed (Id: ..., CorrelationId: ..., Status: Failed)

  ✓ Published PaymentProcessed (Id: ..., CorrelationId: ..., Status: Pending)

Waiting for message processing to complete...

================================================================================
Sample application completed successfully!
================================================================================
```

## Architecture

### Publish/Subscribe Pattern

The application demonstrates the publish/subscribe pattern where multiple handlers can subscribe to the same message type:

```mermaid
graph TB
    subgraph "Message Types"
        MC[OrderCreated]
        UR[UserRegistered]
        PP[PaymentProcessed]
    end
    
    subgraph "OrderCreated Handlers"
        LH1[Logging Handler]
        NH1[Notification Handler]
        IH1[Inventory Handler]
        AH1[Analytics Handler]
    end
    
    subgraph "UserRegistered Handlers"
        LH2[Logging Handler]
        AH2[Analytics Handler]
        WH1[Welcome Email Handler]
        AUD1[Audit Handler]
    end
    
    subgraph "PaymentProcessed Handlers"
        LH3[Logging Handler]
        OH1[Order Status Handler]
        NH2[Receipt Handler]
        AH2[Analytics Handler]
        AUD2[Audit Handler]
    end
    
    MC --> LH1
    MC --> NH1
    MC --> IH1
    MC --> AH1
    
    UR --> LH2
    UR --> AH2
    UR --> WH1
    UR --> AUD1
    
    PP --> LH3
    PP --> OH1
    PP --> NH2
    PP --> AH2
    PP --> AUD2
```

### Correlation ID Flow

Every message includes a `CorrelationId` that is logged by all handlers:

```
[LOGGING HANDLER] Order Created
  CorrelationId: abc123def456...
  OrderId: {Guid}
  ...
```

This enables tracing messages across multiple systems and handlers.

### EasyNetQ DI Integration

The application uses EasyNetQ's built-in dependency injection support:

```csharp
services.AddEasyNetQ(
    connectionFactory: new ConnectionFactory
    {
        Uri = new Uri(connectionString),
        VirtualHost = virtualHost,
        DeserializedMessageVersion = 2
    },
    busConfigurator: bc =>
    {
        bc.UsePubSub(); // Enable Pub/Sub
        bc.UseRabbitMqExchangeNamingStrategy(new ExchangeNamingStrategy());
    }
);
```

## Message Types

### OrderCreated

Represents an order creation event with items and totals.

```csharp
public record OrderCreated(
    Guid Id,
    string CorrelationId,      // For tracking across systems
    string CustomerId,
    List<OrderItem> Items,
    decimal TotalAmount,
    DateTime CreatedAt
);

public record OrderItem(
    string ProductName,
    int Quantity,
    decimal UnitPrice
);
```

### UserRegistered

Represents a user registration event.

```csharp
public record UserRegistered(
    Guid Id,
    string CorrelationId,      // For tracking across systems
    string UserId,
    string Email,
    DateTime RegisteredAt
);
```

### PaymentProcessed

Represents a payment processing event with status.

```csharp
public record PaymentProcessed(
    Guid Id,
    string CorrelationId,      // For tracking across systems
    string PaymentId,
    string OrderId,
    decimal Amount,
    PaymentStatus Status,      // Success | Failed | Pending
    DateTime ProcessedAt
);

public enum PaymentStatus
{
    Success,
    Failed,
    Pending
}
```

## Handler Patterns

### Logging Handler

Simple handler that logs message details to console:

```csharp
public static async Task LogOrderCreatedAsync(OrderCreated message, CancellationToken ct = default)
{
    Console.WriteLine($"[LOGGING HANDLER] Order Created");
    Console.WriteLine($"  CorrelationId: {message.CorrelationId}");
    Console.WriteLine($"  OrderId: {message.Id}");
    // ...
}
```

### Notification Handler

Sends external notifications (email, SMS):

```csharp
public static async Task SendOrderNotificationAsync(OrderCreated message, CancellationToken ct = default)
{
    // In production, this would send an email via SMTP or an email service
    Console.WriteLine($"[NOTIFICATION HANDLER] Sending order notification");
    Console.WriteLine($"  CustomerId: {message.CustomerId}");
    // ...
}
```

### Processing Handler

Performs business logic operations:

```csharp
public static async Task ProcessOrderInventoryAsync(OrderCreated message, CancellationToken ct = default)
{
    // In production, this would call an inventory service
    Console.WriteLine($"[INVENTORY HANDLER] Processing order inventory");
    foreach (var item in message.Items)
    {
        Console.WriteLine($"    Reserving: {item.ProductName} x{item.Quantity}");
    }
}
```

### Analytics Handler

Tracks events for analytics and reporting:

```csharp
public static async Task TrackOrderAnalyticsAsync(OrderCreated message, CancellationToken ct = default)
{
    // In production, this would call an analytics service
    Console.WriteLine($"[ANALYTICS HANDLER] Tracking order creation event");
    Console.WriteLine($"  Event: OrderCreated");
    Console.WriteLine($"  CustomerId: {message.CustomerId}");
}
```

### Audit Handler

Records events in audit logs for compliance:

```csharp
public static async Task LogUserRegistrationAuditAsync(UserRegistered message, CancellationToken ct = default)
{
    // In production, this would write to an audit database/log
    Console.WriteLine($"[AUDIT HANDLER] Recording user registration audit");
    Console.WriteLine($"  UserId: {message.UserId}");
    Console.WriteLine($"  Timestamp: {DateTime.UtcNow:O}");
}
```

## Usage Examples

### Subscribing to Messages

Subscribe multiple handlers to the same message type:

```csharp
await messageBus.SubscribeAsync<OrderCreated>(OrderHandlers.LogOrderCreatedAsync);
await messageBus.SubscribeAsync<OrderCreated>(OrderHandlers.SendOrderNotificationAsync);
await messageBus.SubscribeAsync<OrderCreated>(OrderHandlers.ProcessOrderInventoryAsync);
```

### Publishing Messages

Publish a message with correlation ID:

```csharp
var order = new OrderCreated(
    Id: Guid.NewGuid(),
    CorrelationId: GenerateCorrelationId(),  // Unique tracking ID
    CustomerId: "CUST-12345",
    Items: new List<OrderItem> { /* ... */ },
    TotalAmount: 99.99m,
    CreatedAt: DateTime.UtcNow
);

await messageBus.PublishAsync(order);
```

### Using Cancellation Token

Handlers can accept a cancellation token for cooperative cancellation:

```csharp
public static async Task HandleMessageAsync(MessageType message, CancellationToken ct)
{
    try
    {
        // Long-running operation
        await SomeLongRunningOperation(message, ct);
    }
    catch (OperationCanceledException)
    {
        // Handle cancellation gracefully
        Console.WriteLine($"Operation cancelled: {message.CorrelationId}");
    }
}
```

## Troubleshooting

### Connection Issues

If you see connection errors:

1. Verify RabbitMQ is running: `http://localhost:15672` (management UI)
2. Check environment variables are set correctly
3. Verify the connection string format: `amqp://username:password@host:port/vhost`

### Message Not Being Processed

If messages aren't being processed:

1. Check that handlers are subscribed before publishing
2. Verify the RabbitMQ broker is accessible
3. Check for exceptions in handler code (they won't prevent other handlers from running)

### Handler Exceptions

Handlers can throw exceptions, but this doesn't affect other subscribers:

```csharp
public static async Task FailingHandlerAsync(OrderCreated message, CancellationToken ct = default)
{
    if (message.TotalAmount > 1000)
    {
        throw new InvalidOperationException("Large orders not supported");
    }
}
```

### Enabling Dead Letter Queue

To enable dead letter queue for failed messages:

```csharp
services.AddMessaging(options =>
{
    options.EnableDeadLetter = true;  // Enable DLQ
});
```

## Best Practices

1. **Always include CorrelationId**: Use a unique correlation ID for every message to enable end-to-end tracing.

2. **Handle exceptions gracefully**: Catch and log exceptions in handlers to prevent message loss.

3. **Use cancellation tokens**: Accept `CancellationToken` in handlers to support cooperative cancellation.

4. **Keep handlers simple**: Each handler should have a single responsibility (logging, notification, processing, etc.).

5. **Configure retry policies**: Use EasyNetQ's built-in retry policy for transient failures.

## License

This sample application is provided as-is for demonstration purposes.

## Contributing

Feel free to fork and modify this sample application for your own use cases!
