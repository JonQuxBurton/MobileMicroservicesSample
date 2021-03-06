﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MinimalEventBus.JustSaying;
using MobileTelecomsNetwork.EventHandlers.Data;
using MobileTelecomsNetwork.EventHandlers.Domain;
using MobileTelecomsNetwork.EventHandlers.Messages;

namespace MobileTelecomsNetwork.EventHandlers.Handlers
{
    public class ActivateRequestedHandler : IHandlerAsync<ActivateRequestedMessage>
    {
        private readonly ILogger<ActivateRequestedHandler> logger;
        private readonly IDataStore dataStore;
        private readonly IExternalMobileTelecomsNetworkService externalMobileTelecomsNetworkService;
        private readonly IMessagePublisher messagePublisher;
        private readonly IMonitoring monitoring;

        public ActivateRequestedHandler(ILogger<ActivateRequestedHandler> logger,
            IDataStore dataStore,
            IExternalMobileTelecomsNetworkService externalMobileTelecomsNetworkService,
            IMessagePublisher messagePublisher,
            IMonitoring monitoring
            )
        {
            this.logger = logger;
            this.dataStore = dataStore;
            this.externalMobileTelecomsNetworkService = externalMobileTelecomsNetworkService;
            this.messagePublisher = messagePublisher;
            this.monitoring = monitoring;
        }

        public async Task<bool> Handle(ActivateRequestedMessage receivedEvent)
        {
            var eventName = receivedEvent.GetType().Name;
            logger.LogInformation("Received event [{eventName}] with MobileOrderId {mobileOrderId} ActivationCode {activationCode}", eventName, receivedEvent.MobileOrderId, receivedEvent.ActivationCode);

            try
            {
                using (var tx = dataStore.BeginTransaction())
                {
                    dataStore.Add(new Order
                    {
                        MobileId = receivedEvent.MobileId,
                        PhoneNumber = receivedEvent.PhoneNumber,
                        ActivationCode = receivedEvent.ActivationCode,
                        MobileOrderId = receivedEvent.MobileOrderId,
                        Status = OrderStatus.New,
                        Type = OrderType.Activate,
                    });
                }

                using (var tx = dataStore.BeginTransaction())
                {
                    var result = await externalMobileTelecomsNetworkService.PostOrder(new ExternalMobileTelecomsNetworkOrder
                    {
                        PhoneNumber = receivedEvent.PhoneNumber,
                        Reference = receivedEvent.MobileOrderId,
                        ActivationCode = receivedEvent.ActivationCode
                    });

                    if (!result)
                    {
                        tx.Rollback();
                        monitoring.ActivateOrderFailed();
                        return false;
                    }

                    dataStore.Sent(receivedEvent.MobileOrderId);
                    Publish(receivedEvent.MobileOrderId);
                    monitoring.ActivateOrderSent();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while processing event {eventName} - MobileOrderId={mobileOrderId}", eventName, receivedEvent.MobileOrderId);
                return false;
            }

            return true;
        }

        private void Publish(Guid mobileGlobalId)
        {
            logger.LogInformation("Publishing event [{event}]", typeof(ActivateOrderSentMessage).Name);

            messagePublisher.PublishAsync(new ActivateOrderSentMessage
            {
                MobileOrderId = mobileGlobalId
            });
        }
    }
}
