namespace MinimalEventBus
{
    public class BusBodyMessage
    {
        public string MessageId { get; set; }
        public string Type { get; set; }

        public string TopicArn { get; set; }

        public string Message { get; set; }
    }
}
