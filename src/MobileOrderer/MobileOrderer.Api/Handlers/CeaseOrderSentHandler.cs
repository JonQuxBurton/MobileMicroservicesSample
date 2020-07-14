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
    public class CeaseOrderSentHandler : IHandlerAsync<CeaseOrderSentMessage>
    {
        private readonly ILogger<CeaseOrderSentHandler> logger;
        private readonly IRepository<Mobile> mobileRepository;
        private readonly IGetMobileByOrderIdQuery getMobileByOrderIdQuery;

        public CeaseOrderSentHandler(ILogger<CeaseOrderSentHandler> logger, IRepository<Mobile> mobileRepository, IGetMobileByOrderIdQuery getMobileByOrderIdQuery)
        {
            this.logger = logger;
            this.mobileRepository = mobileRepository;
            this.getMobileByOrderIdQuery = getMobileByOrderIdQuery;
        }

        public Task<bool> Handle(CeaseOrderSentMessage message)
        {
            try
            {
                logger.LogInformation($"Received [CeaseOrderSent] MobileOrderId={message.MobileOrderId}");

                var mobile = this.getMobileByOrderIdQuery.Get(message.MobileOrderId);
                mobile.OrderSent();
                this.mobileRepository.Update(mobile);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error while processing CeaseOrderSentMessage");
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
    }
}
