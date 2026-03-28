using System;
using System.Collections.Generic;

namespace RabbitClientEasyNetQ.Contracts
{
    /// <summary>
    /// Shared message contract for demo: OrderCreated
    /// Placed in the shared library so publisher and subscriber use the exact same type.
    /// </summary>
    public record OrderCreated(
        Guid Id,
        string CorrelationId,
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
}
