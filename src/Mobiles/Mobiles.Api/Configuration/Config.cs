namespace Mobiles.Api.Configuration
{
    public class Config
    {
        public string LogFilePath { get; set; }
        public string SeqUrl { get; set; }
        public string ConnectionString { get; set; }
        public int EventPublisherServicePollingIntervalSeconds { get; set; } = 10;
    }
}