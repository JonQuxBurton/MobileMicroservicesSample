using MinimalEventBus.JustSaying;
using System;

namespace MinimalEventBus
{
    public interface IQueueNamingStrategy
    {
        string GetName(Message message);
        string GetName(Type messageType);
    }
}