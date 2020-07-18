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
    public class CeaseOrderCompletedHandler : IHandlerAsync<CeaseOrderCompletedMessage>
    {
        private readonly ILogger<CeaseOrderCompletedHandler> logger;
        private readonly IRepository<Mobile> mobileRepository;
        private readonly IGetMobileByOrderIdQuery getMobileByOrderIdQuery;
        private readonly IMonitoring monitoring;

        public CeaseOrderCompletedHandler(ILogger<CeaseOrderCompletedHandler> logger, IRepository<Mobile> mobileRepository, IGetMobileByOrderIdQuery getMobileByOrderIdQuery, IMonitoring monitoring)
        {
            this.logger = logger;
            this.mobileRepository = mobileRepository;
            this.getMobileByOrderIdQuery = getMobileByOrderIdQuery;
            this.monitoring = monitoring;
        }

        public Task<bool> Handle(CeaseOrderCompletedMessage message)
        {
            this.logger.LogInformation($"Received [CeaseOrderCompleted] MobileOrderId={message.MobileOrderId}");

            try
            {
                var mobile = getMobileByOrderIdQuery.Get(message.MobileOrderId);
                mobile.CeaseCompleted();
                mobileRepository.Update(mobile);
                monitoring.CeaseCompleted();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error while processing CeaseOrderCompletedMessage");
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
    }
}
