using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MobileOrderer.Api.Services
{
    public class EventPublisherService : BackgroundService
    {
        private readonly ILogger<EventPublisherService> logger;
        private readonly IMobileRequestedEventChecker mobileRequestedEventChecker;

        public EventPublisherService(ILogger<EventPublisherService> logger, IMobileRequestedEventChecker mobileRequestedEventChecker)
        {
            this.logger = logger;
            this.mobileRequestedEventChecker = mobileRequestedEventChecker;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("EventPublisherService is starting...");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    mobileRequestedEventChecker.Check();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Exception");
                }

                await Task.Delay(10 * 1000, stoppingToken);
            }
        }
    }
}
