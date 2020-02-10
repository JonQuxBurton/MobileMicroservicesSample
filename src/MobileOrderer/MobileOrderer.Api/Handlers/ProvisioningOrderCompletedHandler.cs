using Microsoft.Extensions.Logging;
using MinimalEventBus.JustSaying;
using MobileOrderer.Api.Data;
using MobileOrderer.Api.Domain;
using MobileOrderer.Api.Messages;
using System.Threading.Tasks;
using Utils.DomainDrivenDesign;

namespace MobileOrderer.Api.Handlers
{
    public class ProvisioningOrderCompletedHandler : IHandlerAsync<ProvisioningOrderCompletedMessage>
    {
        private readonly ILogger<ProvisioningOrderCompletedHandler> logger;
        private readonly IRepository<Mobile> mobileRepository;
        private readonly IGetMobileByOrderIdQuery getMobileByOrderIdQuery;

        public ProvisioningOrderCompletedHandler(ILogger<ProvisioningOrderCompletedHandler> logger, IRepository<Mobile> mobileRepository, IGetMobileByOrderIdQuery getMobileByOrderIdQuery)
        {
            this.logger = logger;
            this.mobileRepository = mobileRepository;
            this.getMobileByOrderIdQuery = getMobileByOrderIdQuery;
        }

        public Task<bool> Handle(ProvisioningOrderCompletedMessage message)
        {
            this.logger.LogInformation($"Received [ProvisioningOrderCompleted] MobileOrderId={message.MobileOrderId}");

            var mobile = this.getMobileByOrderIdQuery.Get(message.MobileOrderId);
            mobile.ProcessingProvisioningCompleted();
            this.mobileRepository.Update(mobile);

            return Task.FromResult(true);
        }
    }
}
