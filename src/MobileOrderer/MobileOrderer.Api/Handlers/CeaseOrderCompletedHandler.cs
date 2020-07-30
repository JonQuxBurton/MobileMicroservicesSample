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
    public class CeaseOrderCompletedHandler : IHandlerAsync<CeaseOrderCompletedMessage>
    {
        private readonly ILogger<CeaseOrderCompletedHandler> logger;
        private readonly IMonitoring monitoring;
        private readonly IServiceProvider serviceProvider;

        public CeaseOrderCompletedHandler(ILogger<CeaseOrderCompletedHandler> logger,
            IMonitoring monitoring,
            IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.monitoring = monitoring;
            this.serviceProvider = serviceProvider;
        }

        public Task<bool> Handle(CeaseOrderCompletedMessage receivedEvent)
        {
            var eventName = receivedEvent.GetType().Name;
            logger.LogInformation("Received event [{eventName}] - MobileOrderId={MobileOrderId}", eventName, receivedEvent.MobileOrderId);

            try
            {
                using var scope = serviceProvider.CreateScope();
                var getMobileByOrderIdQuery = scope.ServiceProvider.GetRequiredService<IGetMobileByOrderIdQuery>();
                var mobileRepository = scope.ServiceProvider.GetRequiredService<IRepository<Mobile>>();

                var mobile = getMobileByOrderIdQuery.Get(receivedEvent.MobileOrderId);
                mobile.CeaseCompleted();
                mobileRepository.Update(mobile);
                monitoring.CeaseCompleted();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while processing event [{eventName}]", eventName);
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
    }
}
