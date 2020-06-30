﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MinimalEventBus.JustSaying;
using MobileTelecomsNetwork.EventHandlers.Data;
using MobileTelecomsNetwork.EventHandlers.Messages;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MobileTelecomsNetwork.EventHandlers.Services
{
    public class ActivationOrderChecker : IActivationOrderChecker
    {
        private readonly ILogger<CompletedOrderPollingHostedService> logger;
        private readonly IHttpClientFactory clientFactory;
        private readonly IDataStore dataStore;
        private readonly IMessagePublisher messagePublisher;
        private readonly string externalApiUrl;

        public ActivationOrderChecker(
            ILogger<CompletedOrderPollingHostedService> logger,
            IHttpClientFactory clientFactory,
            IDataStore dataStore,
            IMessagePublisher messagePublisher,
            IOptions<Config> config)
        {
            this.logger = logger;
            this.clientFactory = clientFactory;
            this.dataStore = dataStore;
            this.messagePublisher = messagePublisher;
            this.externalApiUrl = config.Value?.ExternalMobileTelecomsNetworkApiUrl;
        }

        public async Task Check(ActivationOrder sentOrder)
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

        private void PublishActivationOrderCompleted(Guid mobileGlobalId)
        {
            logger.LogInformation($"Publishing ActivationOrderCompletedMessage [{mobileGlobalId}]");

            messagePublisher.PublishAsync(new ActivationOrderCompletedMessage
            {
                MobileOrderId = mobileGlobalId
            });
        }
    }
}