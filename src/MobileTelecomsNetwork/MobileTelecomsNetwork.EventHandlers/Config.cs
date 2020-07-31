namespace MobileTelecomsNetwork.EventHandlers
{
    public class Config
    {
        public string LogFilePath { get; set; }
        public string ConnectionString { get; set; }
        public string ExternalMobileTelecomsNetworkApiUrl { get; set; }
        public int MetricsServerHostPort { get; set; }
        public int CompletedOrderPollingIntervalSeconds { get; set; } = 10;
        public int CompletedOrderPollingBatchSize { get; set; } = 10;
    }
}