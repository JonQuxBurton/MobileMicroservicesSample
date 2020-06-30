using MinimalEventBus.JustSaying;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntegrationTests
{
    public class InMemoryMessagePublisher : IMessagePublisher
    {
        public List<Message> Published = new List<Message>();

        public Task PublishAsync(Message message)
        {
            Published.Add(message);

            return Task.CompletedTask;
        }
    }
}
