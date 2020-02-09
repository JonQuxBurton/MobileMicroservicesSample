using Microsoft.Extensions.Logging;
using MinimalEventBus.JustSaying;
using MobileOrderer.Api.Messages;
using System.Threading.Tasks;

namespace MobileOrderer.Api.Handlers
{
    public class OrderProcessedHandler : IHandlerAsync<OrderProcessedMessage>
    {
        private readonly ILogger<OrderProcessedHandler> logger;

        public OrderProcessedHandler(ILogger<OrderProcessedHandler> logger)
        {
            this.logger = logger;
            this.logger = logger;
        }

        public Task<bool> Handle(OrderProcessedMessage message)
        {
            this.logger.LogInformation($"Received [OrderProcessed] MobileOrderId={message.MobileOrderId}");

            return Task.FromResult(true);
        }
    }
}
