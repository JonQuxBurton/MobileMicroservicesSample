using System.Threading.Tasks;

namespace MinimalEventBus.JustSaying
{
    public interface IMessagePublisher
    {
        Task<bool> PublishAsync(Message message);
    }
}
