using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using System;
using SimCards.EventHandlers.Data;
using System.Linq;
using SimCards.EventHandlers.Domain;
using Microsoft.Extensions.Options;

namespace SimCards.EventHandlers.BackgroundServices
{
    public class CompletedOrderPollingHostedService : BackgroundService
    {
        private readonly ILogger<CompletedOrderPollingHostedService> logger;
        private readonly TimeSpan pollingInterval;
        private readonly int batchSize;

        private readonly ISimCardOrdersDataStore simCardOrdersDataStore;
        private readonly ICompletedOrderChecker completedOrderCheker;

        public CompletedOrderPollingHostedService(IOptions<Config> options,
            ILogger<CompletedOrderPollingHostedService> logger,
            ISimCardOrdersDataStore simCardOrdersDataStore,
            ICompletedOrderChecker completedOrderCheker)
        {
            pollingInterval = TimeSpan.FromSeconds(options.Value.CompletedOrderPollingIntervalSeconds);
            batchSize = options.Value.CompletedOrderPollingBatchSize;
            this.logger = logger;
            this.simCardOrdersDataStore = simCardOrdersDataStore;
            this.completedOrderCheker = completedOrderCheker;
        }

        public async Task DoWork()
        {
            var sentOrders = simCardOrdersDataStore.GetSent().Take(batchSize);

            foreach (var sentOrder in sentOrders)
            {
                await completedOrderCheker.Check(sentOrder);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("{ServiceName} starting...", ServiceName);
            
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    logger.LogDebug("{ServiceName} ExecuteAsync...", ServiceName);

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
