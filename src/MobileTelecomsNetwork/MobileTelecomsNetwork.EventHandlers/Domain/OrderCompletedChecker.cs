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

                if (externalOrder.Status == OrderStatus.Completed.ToString())
                {
                    using var tx = dataStore.BeginTransaction();
                    dataStore.Complete(sentOrder.MobileOrderId);

                    if (sentOrder.Type == OrderType.Cease)
                        PublishCeaseOrderCompleted(sentOrder.MobileOrderId);
                    else
                        PublishActivateOrderCompleted(sentOrder.MobileOrderId);
                }
                else if (externalOrder.Status == OrderStatus.Rejected.ToString())
                {
                    using var tx = dataStore.BeginTransaction();
                    dataStore.Complete(sentOrder.MobileOrderId);

                    if (sentOrder.Type == OrderType.Activate)
                        PublishActivateOrderRejected(sentOrder.PhoneNumber, sentOrder.MobileOrderId, externalOrder.Reason);
                }
                else
                {
                    using var tx = dataStore.BeginTransaction();
                    dataStore.IncrementAttempts(sentOrder);
                }
            }
            else
            {
                dataStore.IncrementAttempts(sentOrder);
                logger.LogWarning("Could not get MobileOrder from External SIM Card service - MobileOrderId {MobileOrderId}, response.StatusCode {StatusCode}", sentOrder.MobileOrderId, response.StatusCode);
            }
        }

        private async void PublishActivateOrderCompleted(Guid mobileGlobalId)
        {
            logger.LogInformation("Publishing event [{event}] - MobileId {mobileGlobalId}", nameof(ActivateOrderCompletedMessage), mobileGlobalId);

            var result = await messagePublisher.PublishAsync(new ActivateOrderCompletedMessage
            {
                MobileOrderId = mobileGlobalId
            });

            if (!result)
                logger.LogError("Error while publishing event [{event}] - MobileId {mobileGlobalId}", nameof(ActivateOrderCompletedMessage), mobileGlobalId);
            else
                monitoring.ActivateOrderCompleted();
        }
        
        private async void PublishActivateOrderRejected(string phoneNumber, Guid mobileGlobalId, string reason)
        {
            logger.LogInformation("Publishing event [{event}]", nameof(ActivateOrderRejectedMessage));

            var result = await messagePublisher.PublishAsync(new ActivateOrderRejectedMessage
            {
                PhoneNumber = phoneNumber,
                MobileOrderId = mobileGlobalId,
                Reason = reason
            });

            if (!result)
                logger.LogError("Error while publishing event [{event}] - MobileId {mobileGlobalId}", nameof(ActivateOrderRejectedMessage), mobileGlobalId);
            else
                monitoring.ActivateOrderCompleted();
        }

        private async void PublishCeaseOrderCompleted(Guid mobileGlobalId)
        {
            logger.LogInformation("Publishing event [{event}] - MobileId {mobileGlobalId}", nameof(CeaseOrderCompletedMessage), mobileGlobalId);

            var result = await messagePublisher.PublishAsync(new CeaseOrderCompletedMessage
            {
                MobileOrderId = mobileGlobalId
            });

            if (!result)
                logger.LogError("Error while publishing event [{event}] - MobileId {mobileGlobalId}", nameof(CeaseOrderCompletedMessage), mobileGlobalId);
            else
                monitoring.CeaseOrderCompleted();
        }
    }
}
