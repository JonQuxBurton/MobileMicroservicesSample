using MinimalEventBus.JustSaying;
using System;

namespace MinimalEventBus
{
    public class DefaultQueueNamingStrategy : IQueueNamingStrategy
    {
        public string GetName(Message message)
        {
            return message.GetType().Name.Replace("Message", "");
        }

        public string GetName(Type messageType)
        {
            return messageType.Name.Replace("Message", "");
        }
    }
}
