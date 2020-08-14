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
    public class OrderCompletedHandler : IHandlerAsync<ProvisionOrderCompletedMessage>
    {
        private readonly ILogger<OrderCompletedHandler> logger;
        private readonly IMonitoring monitoring;
        private readonly IServiceProvider serviceProvider;

        public OrderCompletedHandler(
            ILogger<OrderCompletedHandler> logger,
            IMonitoring monitoring,
            IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.monitoring = monitoring;
            this.serviceProvider = serviceProvider;
        }

        public Task<bool> Handle(ProvisionOrderCompletedMessage receivedEvent)
        {
            var eventName = receivedEvent.GetType().Name;
            logger.LogInformation("Received event [{eventName}] - MobileOrderId={MobileOrderId}", eventName, receivedEvent.MobileOrderId);

            try
            {
                using var scope = serviceProvider.CreateScope();
                var getMobileByOrderIdQuery = scope.ServiceProvider.GetRequiredService<IGetMobileByOrderIdQuery>();
                var mobileRepository = scope.ServiceProvider.GetRequiredService<IRepository<Mobile>>();
                var mobile = getMobileByOrderIdQuery.Get(receivedEvent.MobileOrderId);
                
                mobile.ProcessingProvisionCompleted();
                mobileRepository.Update(mobile);
                monitoring.ProvisionCompleted();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while processing event [{eventName}]", eventName);
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
    }
}
