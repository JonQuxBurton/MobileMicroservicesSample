namespace MobileOrderer.Api.Configuration
{
    public class Config
    {
        public string ConnectionString { get; set; }
        public int EventPublisherServicePollingIntervalSeconds { get; set; } = 10;
    }
}