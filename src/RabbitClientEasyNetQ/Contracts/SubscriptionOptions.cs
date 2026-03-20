namespace RabbitClientEasyNetQ.Contracts
{
    public class SubscriptionOptions
    {
        public string? SubscriptionId { get; set; }
        public bool Durable { get; set; } = true;
        public ushort PrefetchCount { get; set; } = 50;
        public int Concurrency { get; set; } = 1;
        public string? DeadLetterQueueName { get; set; }
    }
}
