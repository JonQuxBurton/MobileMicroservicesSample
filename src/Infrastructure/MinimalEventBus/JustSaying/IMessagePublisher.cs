using System.Threading.Tasks;

namespace MinimalEventBus.JustSaying
{
    public interface IMessagePublisher
    {
        Task PublishAsync(Message message);
    }
}
