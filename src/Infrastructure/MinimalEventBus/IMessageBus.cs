using System.Collections.Generic;
using System.Threading.Tasks;
using MinimalEventBus.JustSaying;

namespace MinimalEventBus
{
    public interface IMessageBus
    {
        Dictionary<string, QueueInfo> Queues { get;  }
        void Subscribe<TMessage, THandler>(IHandlerAsync<TMessage> handler)
                where TMessage : Message
                where THandler : class, IHandlerAsync<TMessage>;
        IHandlerAsync<T> GetHandler<T>() where T : Message;
        Task<string> SetupQueue(Message message);
    }
}