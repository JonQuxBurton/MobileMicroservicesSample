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
    public class ActivationOrderCompletedHandler : IHandlerAsync<ActivationOrderCompletedMessage>
    {
        private readonly ILogger<ActivationOrderCompletedHandler> logger;
        private readonly IMonitoring monitoring;
        private readonly IServiceProvider serviceProvider;

        public ActivationOrderCompletedHandler(ILogger<ActivationOrderCompletedHandler> logger,
            IMonitoring monitoring,
            IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.monitoring = monitoring;
            this.serviceProvider = serviceProvider;
        }

        public Task<bool> Handle(ActivationOrderCompletedMessage message)
        {
            try
            {
                logger.LogInformation($"Received [ActivationOrderCompleted] MobileOrderId={message.MobileOrderId}");

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
                logger.LogError(ex, "Error while processing ActivationOrderCompletedMessage");
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
    }
}
