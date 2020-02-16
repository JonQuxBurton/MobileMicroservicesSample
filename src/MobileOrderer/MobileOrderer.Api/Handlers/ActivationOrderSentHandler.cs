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
    public class ActivationOrderSentHandler : IHandlerAsync<ActivationOrderSentMessage>
    {
        private readonly ILogger<ActivationOrderSentHandler> logger;
        private readonly IRepository<Mobile> mobileRepository;
        private readonly IGetMobileByOrderIdQuery getMobileByOrderIdQuery;

        public ActivationOrderSentHandler(ILogger<ActivationOrderSentHandler> logger, IRepository<Mobile> mobileRepository, IGetMobileByOrderIdQuery getMobileByOrderIdQuery)
        {
            this.logger = logger;
            this.mobileRepository = mobileRepository;
            this.getMobileByOrderIdQuery = getMobileByOrderIdQuery;
        }

        public Task<bool> Handle(ActivationOrderSentMessage message)
        {
            try
            {
                this.logger.LogInformation($"Received [ActivationOrderSent] MobileOrderId={message.MobileOrderId}");

                var mobile = this.getMobileByOrderIdQuery.Get(message.MobileOrderId);
                mobile.OrderSent();
                this.mobileRepository.Update(mobile);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error while processing ActivationOrderSentMessage");
            }

            return Task.FromResult(true);
        }
    }
}
