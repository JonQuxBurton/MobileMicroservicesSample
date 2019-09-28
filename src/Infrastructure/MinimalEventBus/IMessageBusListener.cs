using System.Threading.Tasks;
using MinimalEventBus.JustSaying;

namespace MinimalEventBus
{
    public interface IMessageBusListener
    {
        Task CheckForMessages<T>(QueueInfo queue) where T : Message;
        Task StartListening();
    }
}