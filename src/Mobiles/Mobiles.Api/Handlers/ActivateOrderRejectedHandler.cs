using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MinimalEventBus.JustSaying;
using Mobiles.Api.Data;
using Mobiles.Api.Domain;
using Mobiles.Api.Messages;
using System;
using System.Threading.Tasks;
using Utils.DomainDrivenDesign;

namespace Mobiles.Api.Handlers
{
    public class ActivateOrderRejectedHandler : IHandlerAsync<ActivateOrderRejectedMessage>
    {
        private readonly ILogger<ActivateOrderRejectedHandler> logger;
        private readonly IMonitoring monitoring;
        private readonly IServiceProvider serviceProvider;

        public ActivateOrderRejectedHandler(ILogger<ActivateOrderRejectedHandler> logger,
            IMonitoring monitoring,
            IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.monitoring = monitoring;
            this.serviceProvider = serviceProvider;
        }

        public Task<bool> Handle(ActivateOrderRejectedMessage message)
        {
            var messageName = message.GetType().Name;
            logger.LogInformation("Received event [{MessageName}] - MobileOrderId={MobileOrderId}", messageName, message.MobileOrderId);

            try
            {
                using var scope = serviceProvider.CreateScope();
                var getMobileByOrderIdQuery = scope.ServiceProvider.GetRequiredService<IGetMobileByOrderIdQuery>();
                var mobileRepository = scope.ServiceProvider.GetRequiredService<IRepository<Mobile>>();

                var mobile = getMobileByOrderIdQuery.Get(message.MobileOrderId);

                if (mobile == null)
                {
                    logger.LogError("Error while processing event {eventName} - MobileOrderId={mobileOrderId}: Mobile not found", messageName, message.MobileOrderId);
                    return Task.FromResult(false);
                }

                mobile.ActivateRejected();
                mobileRepository.Update(mobile);
                monitoring.ActivateRejected();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while processing event {eventName} - MobileOrderId={mobileOrderId}", messageName, message.MobileOrderId);
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
    }
}
