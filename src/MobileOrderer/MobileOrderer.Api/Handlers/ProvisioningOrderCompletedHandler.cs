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
    public class ProvisioningOrderCompletedHandler : IHandlerAsync<ProvisioningOrderCompletedMessage>
    {
        private readonly ILogger<ProvisioningOrderCompletedHandler> logger;
        private readonly IRepository<Mobile> mobileRepository;
        private readonly IGetMobileByOrderIdQuery getMobileByOrderIdQuery;
        private readonly IMonitoring monitoring;

        public ProvisioningOrderCompletedHandler(ILogger<ProvisioningOrderCompletedHandler> logger, IRepository<Mobile> mobileRepository, IGetMobileByOrderIdQuery getMobileByOrderIdQuery, IMonitoring monitoring)
        {
            this.logger = logger;
            this.mobileRepository = mobileRepository;
            this.getMobileByOrderIdQuery = getMobileByOrderIdQuery;
            this.monitoring = monitoring;
        }

        public Task<bool> Handle(ProvisioningOrderCompletedMessage message)
        {
            this.logger.LogInformation($"Received [ProvisioningOrderCompleted] MobileOrderId={message.MobileOrderId}");

            try
            {
                var mobile = getMobileByOrderIdQuery.Get(message.MobileOrderId);
                mobile.ProcessingProvisioningCompleted();
                mobileRepository.Update(mobile);
                monitoring.ProvisionCompleted();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error while processing ProvisioningOrderCompletedMessage");
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
    }
}
