using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Linq;
using MobileTelecomsNetwork.EventHandlers.Data;
using MobileTelecomsNetwork.EventHandlers.Domain;

namespace MobileTelecomsNetwork.EventHandlers.BackgroundServices
{
    public class CompletedOrderPollingHostedService : BackgroundService
    {
        public const int BatchSize = 10;

        private readonly ILogger<CompletedOrderPollingHostedService> logger;
        private readonly IDataStore dataStore;
        private readonly IOrderCompletedChecker orderCompletedChecker;

        public CompletedOrderPollingHostedService(ILogger<CompletedOrderPollingHostedService> logger,
            IDataStore dataStore,
            IOrderCompletedChecker activationOrderChecker)
        {
            this.logger = logger;
            this.dataStore = dataStore;
            orderCompletedChecker = activationOrderChecker;
        }

        public async void DoWork()
        {
            try
            {
                var orders = dataStore.GetSent().Take(BatchSize);

                foreach (var sentOrder in orders)
                {
                    await orderCompletedChecker.Check(sentOrder);
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
