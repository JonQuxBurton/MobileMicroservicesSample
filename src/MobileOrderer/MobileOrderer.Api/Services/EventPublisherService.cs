using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MobileOrderer.Api.Services
{
    public class EventPublisherService : BackgroundService
    {
        private readonly ILogger<EventPublisherService> logger;
        private readonly IEnumerable<IMobileEventsChecker> checkers;

        public EventPublisherService(ILogger<EventPublisherService> logger, 
            IEnumerable<IMobileEventsChecker> checkers)
        {
            this.logger = logger;
            this.checkers = checkers;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("EventPublisherService is starting...");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    foreach (var checker in checkers)
                    {
                        logger.LogInformation($"Checking {checker.GetType().Name}");
                        checker.Check();
                    }
                            
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
