using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MinimalEventBus.JustSaying;
using MobileTelecomsNetwork.EventHandlers.BackgroundServices;
using MobileTelecomsNetwork.EventHandlers.Data;
using MobileTelecomsNetwork.EventHandlers.Messages;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MobileTelecomsNetwork.EventHandlers.Domain
{
    public class OrderCompletedChecker : IOrderCompletedChecker
    {
        private readonly ILogger<CompletedOrderPollingHostedService> logger;
        private readonly IHttpClientFactory clientFactory;
        private readonly IDataStore dataStore;
        private readonly IMessagePublisher messagePublisher;
        private readonly IMonitoring monitoring;
        private readonly string externalApiUrl;

        public OrderCompletedChecker(
            ILogger<CompletedOrderPollingHostedService> logger,
            IHttpClientFactory clientFactory,
            IDataStore dataStore,
            IMessagePublisher messagePublisher,
            IOptions<Config> config,
            IMonitoring monitoring)
        {
            this.logger = logger;
            this.clientFactory = clientFactory;
            this.dataStore = dataStore;
            this.messagePublisher = messagePublisher;
            this.monitoring = monitoring;
            externalApiUrl = config.Value?.ExternalMobileTelecomsNetworkApiUrl;
        }

        public async Task Check(Order sentOrder)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{externalApiUrl}/api/orders/{sentOrder.MobileOrderId}");
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

                    if (sentOrder.Type.Trim() == "Cease")
                        PublishCeaseOrderCompleted(sentOrder.MobileOrderId);
                    else
                        PublishActivationOrderCompleted(sentOrder.MobileOrderId);
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
            monitoring.ActivateOrderCompleted();
        }

        private void PublishCeaseOrderCompleted(Guid mobileGlobalId)
        {
            logger.LogInformation($"Publishing CeaseOrderCompletedMessage [{mobileGlobalId}]");

            messagePublisher.PublishAsync(new CeaseOrderCompletedMessage
            {
                MobileOrderId = mobileGlobalId
            });
            monitoring.CeaseOrderCompleted();
        }
    }
}
