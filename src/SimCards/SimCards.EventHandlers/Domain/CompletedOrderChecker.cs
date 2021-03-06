﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MinimalEventBus.JustSaying;
using SimCards.EventHandlers.Data;
using SimCards.EventHandlers.Messages;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SimCards.EventHandlers.Domain
{
    public class CompletedOrderChecker : ICompletedOrderChecker
    {
        private readonly ILogger<CompletedOrderChecker> logger;
        private readonly IHttpClientFactory clientFactory;
        private readonly ISimCardOrdersDataStore simCardOrdersDataStore;
        private readonly IMessagePublisher messagePublisher;
        private readonly IMonitoring monitoring;
        private readonly string externalApiUrl;

        public CompletedOrderChecker(ILogger<CompletedOrderChecker> logger,
                                IHttpClientFactory clientFactory,
                                ISimCardOrdersDataStore simCardOrdersDataStore,
                                IMessagePublisher messagePublisher,
                                IOptions<Config> config,
                                IMonitoring monitoring
        )
        {
            this.logger = logger;
            this.clientFactory = clientFactory;
            this.simCardOrdersDataStore = simCardOrdersDataStore;
            this.messagePublisher = messagePublisher;
            this.monitoring = monitoring;
            externalApiUrl = config.Value?.ExternalSimCardsProviderApiUrl;
        }

        public async Task Check(SimCardOrder sentOrder)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{externalApiUrl}/api/orders/{sentOrder.MobileOrderId}");
            var client = clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                var simCardOrderFromExternalProvider = await JsonSerializer.DeserializeAsync<SimCardOrderFromExternalProvider>(responseStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (simCardOrderFromExternalProvider.Status.Trim() == "Completed")
                {
                    using var tx = simCardOrdersDataStore.BeginTransaction();
                    simCardOrdersDataStore.Complete(sentOrder.MobileOrderId);
                    PublishProvisioningOrderCompleted(sentOrder.MobileOrderId);
                    monitoring.SimCardOrderCompleted();
                    simCardOrdersDataStore.IncrementAttempts(sentOrder);
                }
                else
                {
                    using var tx = simCardOrdersDataStore.BeginTransaction();
                    simCardOrdersDataStore.IncrementAttempts(sentOrder);
                }
                
            }
            else
            {
                using var tx = simCardOrdersDataStore.BeginTransaction();
                simCardOrdersDataStore.IncrementAttempts(sentOrder);
            }
        }

        private void PublishProvisioningOrderCompleted(Guid mobileGlobalId)
        {
            logger.LogInformation("Publishing event [{event}] - MobileId {mobileGlobalId}", typeof(ProvisionOrderCompletedMessage).Name, mobileGlobalId);

            messagePublisher.PublishAsync(new ProvisionOrderCompletedMessage
            {
                MobileOrderId = mobileGlobalId
            });
        }
    }
}
