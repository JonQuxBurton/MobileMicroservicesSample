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
    public class OrderSentHandler : IHandlerAsync<OrderSentMessage>
    {
        private readonly ILogger<OrderSentHandler> logger;
        private readonly IRepository<Mobile> mobileRepository;
        private readonly IGetMobileByOrderIdQuery getMobileByOrderIdQuery;

        public OrderSentHandler(ILogger<OrderSentHandler> logger, IRepository<Mobile> mobileRepository, IGetMobileByOrderIdQuery getMobileByOrderIdQuery)
        {
            this.logger = logger;
            this.mobileRepository = mobileRepository;
            this.getMobileByOrderIdQuery = getMobileByOrderIdQuery;
        }

        public Task<bool> Handle(OrderSentMessage message)
        {
            this.logger.LogInformation($"Received [OrderSent] MobileOrderId={message.MobileOrderId}");

            try
            {
                var mobile = this.getMobileByOrderIdQuery.Get(message.MobileOrderId);
                mobile.OrderSent();
                this.mobileRepository.Update(mobile);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error while processing OrderSentMessage");
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
    }
}
