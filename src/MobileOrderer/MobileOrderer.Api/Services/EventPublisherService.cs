using Microsoft.Extensions.DependencyInjection;
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
        private readonly IServiceScopeFactory scopeFactory;

        public EventPublisherService(ILogger<EventPublisherService> logger, IServiceScopeFactory scopeFactory)
        {
            this.logger = logger;
            this.scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                logger.LogInformation("EventPublisherService is starting...");
                var mobileRequestedEventChecker = scope.ServiceProvider.GetRequiredService<IMobileRequestedEventChecker>();

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
}
