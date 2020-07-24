using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using System;
using SimCards.EventHandlers.Data;
using System.Linq;
using SimCards.EventHandlers.Domain;

namespace SimCards.EventHandlers.BackgroundServices
{
    public class CompletedOrderPollingHostedService : BackgroundService
    {
        public const int BatchSize = 10;

        private readonly ILogger<CompletedOrderPollingHostedService> logger;
        private readonly ISimCardOrdersDataStore simCardOrdersDataStore;
        private readonly ICompletedOrderChecker completedOrderCheker;

        public CompletedOrderPollingHostedService(ILogger<CompletedOrderPollingHostedService> logger,
            ISimCardOrdersDataStore simCardOrdersDataStore,
            ICompletedOrderChecker completedOrderCheker)
        {
            this.logger = logger;
            this.simCardOrdersDataStore = simCardOrdersDataStore;
            this.completedOrderCheker = completedOrderCheker;
        }

        public async void DoWork()
        {
            try
            {
                var sentOrders = simCardOrdersDataStore.GetSent().Take(BatchSize);

                foreach (var sentOrder in sentOrders)
                {
                    await completedOrderCheker.Check(sentOrder);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("CompletedOrderPollingHostedService executing...");

            while (!stoppingToken.IsCancellationRequested)
            {
                DoWork();

                await Task.Delay(10 * 1000, stoppingToken);
            }
        }
    }
}
