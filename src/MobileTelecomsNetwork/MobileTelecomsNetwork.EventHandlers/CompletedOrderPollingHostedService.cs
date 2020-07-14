using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Linq;
using MobileTelecomsNetwork.EventHandlers.Data;
using MobileTelecomsNetwork.EventHandlers.Services;

namespace MobileTelecomsNetwork.EventHandlers
{
    public class CompletedOrderPollingHostedService : IHostedService, IDisposable
    {
        public const int BatchSize = 10;

        private readonly ILogger<CompletedOrderPollingHostedService> logger;
        private readonly IDataStore dataStore;
        private readonly IOrderCompletedChecker activationOrderChecker;
        private Timer timer;

        public CompletedOrderPollingHostedService(ILogger<CompletedOrderPollingHostedService> logger,
            IDataStore dataStore,
            IOrderCompletedChecker activationOrderChecker)
        {
            this.logger = logger;
            this.dataStore = dataStore;
            this.activationOrderChecker = activationOrderChecker;
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
                var sentOrders = dataStore.GetSent().Take(BatchSize);

                foreach (var sentOrder in sentOrders)
                {
                    await activationOrderChecker.Check(sentOrder);
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
