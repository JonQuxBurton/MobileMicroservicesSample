using MinimalEventBus.Aws;
using MinimalEventBus.JustSaying;
using System.Threading.Tasks;

namespace MinimalEventBus
{
    public class MessagePublisher : IMessagePublisher
    {
        private readonly IMessageBus messageBus;
        private readonly ISnsService snsService;

        public MessagePublisher(IMessageBus messageBus, ISnsService snsService)
        {
            this.messageBus = messageBus;
            this.snsService = snsService;
        }

        public async Task PublishAsync(Message message)
        {
            var queueName = await this.messageBus.SetupQueue(message);
            await this.snsService.PublishAsync(queueName, message);
        }
    }
}
