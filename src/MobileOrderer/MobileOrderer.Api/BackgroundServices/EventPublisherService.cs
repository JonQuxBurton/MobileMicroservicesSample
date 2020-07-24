using Microsoft.Extensions.DependencyInjection;
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
        private readonly IServiceProvider serviceProvider;

        public EventPublisherService(ILogger<EventPublisherService> logger, 
            IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("EventPublisherService is starting...");

            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = serviceProvider.CreateScope();

                var checkers = scope.ServiceProvider.GetRequiredService<IEnumerable<IMobileEventsChecker>>();

                try
                {
                    foreach (var checker in checkers)
                    {
                        logger.LogInformation($"Checking for events with {checker.GetType().Name}...");
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
