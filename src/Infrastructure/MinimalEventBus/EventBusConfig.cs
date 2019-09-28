namespace MinimalEventBus
{
    public class EventBusConfig
    {
        public string SnsServiceUrl { get; set; }
        public string SqsServiceUrl { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
    }
}
