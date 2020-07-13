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
    public class CancelOrderSentHandler : IHandlerAsync<CancelOrderSentMessage>
    {
        private readonly ILogger<CancelOrderSentHandler> logger;
        private readonly IRepository<Mobile> mobileRepository;
        private readonly IGetMobileByOrderIdQuery getMobileByOrderIdQuery;

        public CancelOrderSentHandler(ILogger<CancelOrderSentHandler> logger, IRepository<Mobile> mobileRepository, IGetMobileByOrderIdQuery getMobileByOrderIdQuery)
        {
            this.logger = logger;
            this.mobileRepository = mobileRepository;
            this.getMobileByOrderIdQuery = getMobileByOrderIdQuery;
        }

        public Task<bool> Handle(CancelOrderSentMessage message)
        {
            try
            {
                logger.LogInformation($"Received [CancelOrderSent] MobileOrderId={message.MobileOrderId}");

                var mobile = this.getMobileByOrderIdQuery.Get(message.MobileOrderId);
                mobile.OrderSent();
                this.mobileRepository.Update(mobile);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error while processing CancelOrderSentMessage");
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
    }
}
