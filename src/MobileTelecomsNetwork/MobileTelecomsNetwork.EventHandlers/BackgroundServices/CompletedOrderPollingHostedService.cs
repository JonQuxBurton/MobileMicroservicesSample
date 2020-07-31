using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Linq;
using MobileTelecomsNetwork.EventHandlers.Data;
using MobileTelecomsNetwork.EventHandlers.Domain;
using Microsoft.Extensions.Options;

namespace MobileTelecomsNetwork.EventHandlers.BackgroundServices
{
    public class CompletedOrderPollingHostedService : BackgroundService
    {
        private readonly ILogger<CompletedOrderPollingHostedService> logger;
        private readonly TimeSpan pollingInterval;
        private readonly int batchSize;
        
        private readonly IDataStore dataStore;
        private readonly IOrderCompletedChecker orderCompletedChecker;

        public CompletedOrderPollingHostedService(IOptions<Config> options, 
            ILogger<CompletedOrderPollingHostedService> logger,
            IDataStore dataStore,
            IOrderCompletedChecker activatesOrderChecker)
        {
            pollingInterval = TimeSpan.FromSeconds(options.Value.CompletedOrderPollingIntervalSeconds);
            batchSize = options.Value.CompletedOrderPollingBatchSize;
            this.logger = logger;
            this.dataStore = dataStore;
            orderCompletedChecker = activatesOrderChecker;
        }

        public async Task DoWork()
        {
            var orders = dataStore.GetSent().Take(batchSize);

            foreach (var sentOrder in orders)
            {
                await orderCompletedChecker.Check(sentOrder);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("{ServiceName} starting...", ServiceName);

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        await DoWork();

                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error when {ServiceName} ExecuteAsync", ServiceName);
                    }

                    await Task.Delay(pollingInterval, stoppingToken);
                }
            }
            catch (TaskCanceledException)
            { }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error when {ServiceName} stopping", ServiceName);
            }

            logger.LogInformation("{ServiceName} stopping...", ServiceName);
        }

        private string ServiceName => GetType().Name;
    }
}
