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
    public class ActivationOrderCompletedHandler : IHandlerAsync<ActivationOrderCompletedMessage>
    {
        private readonly ILogger<ActivationOrderCompletedHandler> logger;
        private readonly IRepository<Mobile> mobileRepository;
        private readonly IGetMobileByOrderIdQuery getMobileByOrderIdQuery;

        public ActivationOrderCompletedHandler(ILogger<ActivationOrderCompletedHandler> logger, IRepository<Mobile> mobileRepository, IGetMobileByOrderIdQuery getMobileByOrderIdQuery)
        {
            this.logger = logger;
            this.mobileRepository = mobileRepository;
            this.getMobileByOrderIdQuery = getMobileByOrderIdQuery;
        }

        public Task<bool> Handle(ActivationOrderCompletedMessage message)
        {
            this.logger.LogInformation($"Received [ActivationOrderCompleted] MobileOrderId={message.MobileOrderId}");

            try
            {
                var mobile = this.getMobileByOrderIdQuery.Get(message.MobileOrderId);
                mobile.ActivateCompleted();
                this.mobileRepository.Update(mobile);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error while processing ActivationOrderCompletedMessage");
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
    }
}
