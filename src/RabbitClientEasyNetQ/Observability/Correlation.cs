using System;

namespace RabbitClientEasyNetQ.Observability
{
    public static class Correlation
    {
        public const string CorrelationIdHeader = "X-Correlation-Id";

        public static string NewId() => Guid.NewGuid().ToString("N");
    }
}
