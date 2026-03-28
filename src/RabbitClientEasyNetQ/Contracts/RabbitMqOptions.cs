using System.Collections.Generic;

namespace RabbitClientEasyNetQ.Contracts
{
    public class RabbitMqConfigs
    {
        public List<RabbitMqOptions> Configs { get; set; } = new List<RabbitMqOptions>();
    }
    public class RabbitMqOptions
    {
        public string? ConnectionString { get; set; }
        public string? HostName { get; set; }
        public string? VirtualHost { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }

        // Resilience
        public int RetryCount { get; set; } = 3;
        public bool EnableDeadLetter { get; set; } = false;

        // Conventions / advanced
        public string? ExchangeNameConvention { get; set; }
    }
}
