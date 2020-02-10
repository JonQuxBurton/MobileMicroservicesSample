using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Net.Http;
using SimCards.EventHandlers.Data;
using System.Linq;
using System.Text.Json;
using MinimalEventBus.JustSaying;

namespace SimCards.EventHandlers
{
    public class CompletedOrderPollingHostedService : IHostedService, IDisposable
    {
        public const int BatchSize = 10;

        private readonly ILogger<CompletedOrderPollingHostedService> logger;
        private readonly IHttpClientFactory clientFactory;
        private readonly ISimCardOrdersDataStore simCardOrdersDataStore;
        private readonly IMessagePublisher messagePublisher;
        private Timer timer;

        public CompletedOrderPollingHostedService(ILogger<CompletedOrderPollingHostedService> logger, IHttpClientFactory clientFactory, ISimCardOrdersDataStore simCardOrdersDataStore,
            IMessagePublisher messagePublisher)
        {
            this.logger = logger;
            this.clientFactory = clientFactory;
            this.simCardOrdersDataStore = simCardOrdersDataStore;
            this.messagePublisher = messagePublisher;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            this.logger.LogInformation("CompletedOrderPollingHostedService Starting...");
            timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            var sentOrders = simCardOrdersDataStore.GetSent().Take(BatchSize);

            foreach (var sentOrder in sentOrders)
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"http://localhost:5001/api/orders/{sentOrder.MobileOrderId}");
                var client = clientFactory.CreateClient();

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    using var responseStream = await response.Content.ReadAsStreamAsync();
                    var simCardOrderFromWholesaler = await JsonSerializer.DeserializeAsync<SimCardOrderFromWholesaler>(responseStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (simCardOrderFromWholesaler.Status.Trim() == "Completed")
                    {
                        using var tx = simCardOrdersDataStore.BeginTransaction();
                        simCardOrdersDataStore.Complete(sentOrder.MobileOrderId);
                        this.PublishProvisioningOrderCompleted(sentOrder.MobileOrderId);
                    }
                }
            }
        }
        private void PublishProvisioningOrderCompleted(Guid mobileGlobalId)
        {
            this.logger.LogInformation($"Publishing ProvisioningOrderCompletedMessage [{mobileGlobalId}]");

            messagePublisher.PublishAsync(new ProvisioningOrderCompletedMessage
            {
                MobileOrderId = mobileGlobalId
            });
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
