using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using System;
using SimCards.EventHandlers.Data;
using System.Linq;

namespace SimCards.EventHandlers
{
    public class CompletedOrderPollingHostedService : IHostedService, IDisposable
    {
        public const int BatchSize = 10;

        private readonly ILogger<CompletedOrderPollingHostedService> logger;
        private readonly ISimCardOrdersDataStore simCardOrdersDataStore;
        private readonly ICompletedOrderChecker completedOrderCheker;
        private Timer timer;

        public CompletedOrderPollingHostedService(ILogger<CompletedOrderPollingHostedService> logger,
            ISimCardOrdersDataStore simCardOrdersDataStore,
            ICompletedOrderChecker completedOrderCheker)
        {
            this.logger = logger;
            this.simCardOrdersDataStore = simCardOrdersDataStore;
            this.completedOrderCheker = completedOrderCheker;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            this.logger.LogInformation("CompletedOrderPollingHostedService Starting...");
            timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        public async void DoWork(object state)
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

        public Task StopAsync(CancellationToken stoppingToken)
        {
            this.logger.LogInformation("CompletedOrderPollingHostedService Stopping...");
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}
