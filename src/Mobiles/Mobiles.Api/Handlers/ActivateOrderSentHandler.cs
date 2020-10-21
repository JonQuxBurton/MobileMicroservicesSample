using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MinimalEventBus.JustSaying;
using Mobiles.Api.Data;
using Mobiles.Api.Domain;
using Mobiles.Api.Messages;
using System;
using System.Threading.Tasks;
using Utils.DomainDrivenDesign;

namespace Mobiles.Api.Handlers
{
    public class ActivateOrderSentHandler : IHandlerAsync<ActivateOrderSentMessage>
    {
        private readonly ILogger<ActivateOrderSentHandler> logger;
        private readonly IServiceProvider serviceProvider;

        public ActivateOrderSentHandler(ILogger<ActivateOrderSentHandler> logger,
            IServiceProvider serviceProvider
            )
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
        }

        public Task<bool> Handle(ActivateOrderSentMessage receivedEvent)
        {
            var eventName = receivedEvent.GetType().Name;
            logger.LogInformation("Received event [{eventName}] - MobileOrderId={MobileOrderId}", eventName, receivedEvent.MobileOrderId);

            try
            {
                using var scope = serviceProvider.CreateScope();
                var getMobileByOrderIdQuery = scope.ServiceProvider.GetRequiredService<IGetMobileByOrderIdQuery>();
                var mobileRepository = scope.ServiceProvider.GetRequiredService<IRepository<Mobile>>();
            
                var mobile = getMobileByOrderIdQuery.Get(receivedEvent.MobileOrderId);

                if (mobile == null)
                {
                    logger.LogError("Error while processing event {eventName} - MobileOrderId={mobileOrderId}: Mobile not found", eventName, receivedEvent.MobileOrderId);
                    return Task.FromResult(false);
                }

                mobile.OrderSent();
                mobileRepository.Update(mobile);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while processing event {eventName} - MobileOrderId={mobileOrderId}", eventName, receivedEvent.MobileOrderId);
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
    }
}
