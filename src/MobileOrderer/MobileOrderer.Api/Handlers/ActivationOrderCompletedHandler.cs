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
        private readonly IMonitoring monitoring;

        public ActivationOrderCompletedHandler(ILogger<ActivationOrderCompletedHandler> logger, IRepository<Mobile> mobileRepository, IGetMobileByOrderIdQuery getMobileByOrderIdQuery, IMonitoring monitoring)
        {
            this.logger = logger;
            this.mobileRepository = mobileRepository;
            this.getMobileByOrderIdQuery = getMobileByOrderIdQuery;
            this.monitoring = monitoring;
        }

        public Task<bool> Handle(ActivationOrderCompletedMessage message)
        {
            this.logger.LogInformation($"Received [ActivationOrderCompleted] MobileOrderId={message.MobileOrderId}");

            try
            {
                var mobile = getMobileByOrderIdQuery.Get(message.MobileOrderId);
                mobile.ActivateCompleted();
                mobileRepository.Update(mobile);
                monitoring.ActivateCompleted();
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
