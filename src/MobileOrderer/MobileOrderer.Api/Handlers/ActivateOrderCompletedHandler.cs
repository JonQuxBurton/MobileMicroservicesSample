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
    public class ActivateOrderCompletedHandler : IHandlerAsync<ActivateOrderCompletedMessage>
    {
        private readonly ILogger<ActivateOrderCompletedHandler> logger;
        private readonly IMonitoring monitoring;
        private readonly IServiceProvider serviceProvider;

        public ActivateOrderCompletedHandler(ILogger<ActivateOrderCompletedHandler> logger,
            IMonitoring monitoring,
            IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.monitoring = monitoring;
            this.serviceProvider = serviceProvider;
        }

        public Task<bool> Handle(ActivateOrderCompletedMessage message)
        {
            var messageName = message.GetType().Name;
            logger.LogInformation($"Received [{messageName}] MobileOrderId={message.MobileOrderId}");

            try
            {

                using var scope = serviceProvider.CreateScope();
                var getMobileByOrderIdQuery = scope.ServiceProvider.GetRequiredService<IGetMobileByOrderIdQuery>();
                var mobileRepository = scope.ServiceProvider.GetRequiredService<IRepository<Mobile>>();

                var mobile = getMobileByOrderIdQuery.Get(message.MobileOrderId);
                mobile.ActivateCompleted();
                mobileRepository.Update(mobile);
                monitoring.ActivateCompleted();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error while processing {messageName}");
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
    }
}
