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
    public class CompletedOrderPollingHostedService : IHostedService, IDisposable
    {
        public const int BatchSize = 10;

        private readonly ILogger<CompletedOrderPollingHostedService> logger;
        private readonly IDataStore dataStore;
        private readonly IOrderCompletedChecker orderCompletedChecker;
        private Timer timer;

        public CompletedOrderPollingHostedService(ILogger<CompletedOrderPollingHostedService> logger,
            IDataStore dataStore,
            IOrderCompletedChecker activationOrderChecker)
        {
            this.logger = logger;
            this.dataStore = dataStore;
            orderCompletedChecker = activationOrderChecker;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("CompletedOrderPollingHostedService Starting...");
            timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        public async void DoWork(object state)
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

        public Task StopAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("CompletedOrderPollingHostedService Stopping...");
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}
