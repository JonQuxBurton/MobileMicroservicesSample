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
    public class ProvisionOrderCompletedHandler : IHandlerAsync<ProvisioningOrderCompletedMessage>
    {
        private readonly ILogger<ProvisionOrderCompletedHandler> logger;
        private readonly IMonitoring monitoring;
        private readonly IServiceProvider serviceProvider;

        public ProvisionOrderCompletedHandler(
            ILogger<ProvisionOrderCompletedHandler> logger,
            IMonitoring monitoring,
            IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.monitoring = monitoring;
            this.serviceProvider = serviceProvider;
        }

        public Task<bool> Handle(ProvisioningOrderCompletedMessage message)
        {
            try
            {
                logger.LogInformation($"Received [ProvisionOrderCompleted] MobileOrderId={message.MobileOrderId}");

                using var scope = serviceProvider.CreateScope();
                var getMobileByOrderIdQuery = scope.ServiceProvider.GetRequiredService<IGetMobileByOrderIdQuery>();
                var mobileRepository = scope.ServiceProvider.GetRequiredService<IRepository<Mobile>>();
                var mobile = getMobileByOrderIdQuery.Get(message.MobileOrderId);
                
                mobile.ProcessingProvisioningCompleted();
                mobileRepository.Update(mobile);
                monitoring.ProvisionCompleted();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while processing ProvisioningOrderCompletedMessage");
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
    }
}
