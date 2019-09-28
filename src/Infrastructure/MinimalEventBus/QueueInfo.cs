using System;

namespace MinimalEventBus
{
    public class QueueInfo
    {
        public QueueInfo(Type messageType, string url)
        {
            MessageType = messageType;
            Url = url;
        }

        public Type MessageType { get; }
        public string Url { get; }
    }
}
