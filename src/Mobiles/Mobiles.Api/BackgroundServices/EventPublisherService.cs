using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mobiles.Api.Configuration;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Mobiles.Api.Services
{
    public class EventPublisherService : BackgroundService
    {
        private readonly ILogger<EventPublisherService> logger;
        private readonly IServiceProvider serviceProvider;
        private readonly TimeSpan pollingInterval;

        public EventPublisherService(IOptions<Config> options, 
            ILogger<EventPublisherService> logger, 
            IServiceProvider serviceProvider)
        {
            pollingInterval = TimeSpan.FromSeconds(options.Value.EventPublisherServicePollingIntervalSeconds);
            this.logger = logger;
            this.serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("{ServiceName} starting...", ServiceName);

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    using var scope = serviceProvider.CreateScope();
                    var checkers = scope.ServiceProvider.GetRequiredService<IEnumerable<IMobileEventsChecker>>();

                    try
                    {
                        foreach (var checker in checkers)
                        {
                            logger.LogDebug("Checking for events with {checker}...", checker.GetType().Name);
                            checker.Check();
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error when checking");
                    }

                    await Task.Delay(pollingInterval, stoppingToken);
                }
            }
            catch (TaskCanceledException)
            {
                logger.LogInformation("{ServiceName} stopping...", ServiceName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error when {ServiceName} ExecuteAsync", ServiceName);
            }
        }

        private string ServiceName => GetType().Name;
    }
}
