using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Net.Http;
using System.Linq;
using System.Text.Json;
using MinimalEventBus.JustSaying;
using MobileTelecomsNetwork.EventHandlers.Data;
using Microsoft.Extensions.Options;
using MobileTelecomsNetwork.EventHandlers.Messages;

namespace MobileTelecomsNetwork.EventHandlers
{
    public class CompletedOrderPollingHostedService : IHostedService, IDisposable
    {
        public const int BatchSize = 10;

        private readonly ILogger<CompletedOrderPollingHostedService> logger;
        private readonly IHttpClientFactory clientFactory;
        private readonly IDataStore dataStore;
        private readonly IMessagePublisher messagePublisher;
        private Timer timer;
        private readonly string externalApiUrl;

        public CompletedOrderPollingHostedService(ILogger<CompletedOrderPollingHostedService> logger, IHttpClientFactory clientFactory, IDataStore dataStore,
            IMessagePublisher messagePublisher,
            IOptions<Config> config)
        {
            this.logger = logger;
            this.clientFactory = clientFactory;
            this.dataStore = dataStore;
            this.messagePublisher = messagePublisher;
            this.externalApiUrl = config.Value?.ExternalMobileTelecomsNetworkApiUrl;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("CompletedOrderPollingHostedService Starting...");
            timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            var sentOrders = dataStore.GetSent().Take(BatchSize);

            foreach (var sentOrder in sentOrders)
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"{this.externalApiUrl}/api/orders/{sentOrder.MobileOrderId}");
                var client = clientFactory.CreateClient();

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    using var responseStream = await response.Content.ReadAsStreamAsync();
                    var externalOrder = await JsonSerializer.DeserializeAsync<ExternalOrder>(responseStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (externalOrder.Status.Trim() == "Completed")
                    {
                        using var tx = dataStore.BeginTransaction();
                        dataStore.Complete(sentOrder.MobileOrderId);
                        this.PublishActivationOrderCompleted(sentOrder.MobileOrderId);
                    }
                }
            }
        }
        private void PublishActivationOrderCompleted(Guid mobileGlobalId)
        {
            logger.LogInformation($"Publishing ActivationOrderCompletedMessage [{mobileGlobalId}]");

            messagePublisher.PublishAsync(new ActivationOrderCompletedMessage
            {
                MobileOrderId = mobileGlobalId
            });
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
