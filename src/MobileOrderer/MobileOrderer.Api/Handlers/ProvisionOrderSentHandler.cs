using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MinimalEventBus.JustSaying;
using MobileOrderer.Api.Data;
using MobileOrderer.Api.Domain;
using MobileOrderer.Api.Messages;
using System;
using System.Threading.Tasks;
using Utils.DomainDrivenDesign;

namespace MobileOrderer.Api.Handlers
{
    public class ProvisionOrderSentHandler : IHandlerAsync<OrderSentMessage>
    {
        private readonly ILogger<ProvisionOrderSentHandler> logger;
        private readonly IServiceProvider serviceProvider;

        public ProvisionOrderSentHandler(ILogger<ProvisionOrderSentHandler> logger,
            IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
        }

        public Task<bool> Handle(OrderSentMessage message)
        {
            try
            {
                logger.LogInformation($"Received [ProvisionOrderSent] MobileOrderId={message.MobileOrderId}");

                using var scope = serviceProvider.CreateScope();
                var getMobileByOrderIdQuery = scope.ServiceProvider.GetRequiredService<IGetMobileByOrderIdQuery>();
                var mobileRepository = scope.ServiceProvider.GetRequiredService<IRepository<Mobile>>();
         
                var mobile = getMobileByOrderIdQuery.Get(message.MobileOrderId);
                mobile.OrderSent();
                mobileRepository.Update(mobile);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while processing OrderSentMessage");
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
    }
}
