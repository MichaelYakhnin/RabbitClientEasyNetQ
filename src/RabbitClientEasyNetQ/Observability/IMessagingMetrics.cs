namespace RabbitClientEasyNetQ.Observability
{
    public interface IMessagingMetrics
    {
        void IncrementPublished(string messageType);
        void IncrementConsumed(string messageType);
    }
}
