﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MinimalEventBus.JustSaying;
using MobileTelecomsNetwork.EventHandlers.Data;
using MobileTelecomsNetwork.EventHandlers.Messages;
using MobileTelecomsNetwork.EventHandlers.Services;

namespace MobileTelecomsNetwork.EventHandlers.Handlers
{
    public class CancelRequestedHandler : IHandlerAsync<CancelRequestedMessage>
    {
        private readonly ILogger<CancelRequestedHandler> logger;
        private readonly IDataStore dataStore;
        private readonly IExternalMobileTelecomsNetworkService externalMobileTelecomsNetworkService;
        private readonly IMessagePublisher messagePublisher;

        public CancelRequestedHandler(ILogger<CancelRequestedHandler> logger, 
            IDataStore dataStore,
            IExternalMobileTelecomsNetworkService externalMobileTelecomsNetworkService,
            IMessagePublisher messagePublisher
            )
        {
            this.logger = logger;
            this.dataStore = dataStore;
            this.externalMobileTelecomsNetworkService = externalMobileTelecomsNetworkService;
            this.messagePublisher = messagePublisher;
        }

        public async Task<bool> Handle(CancelRequestedMessage message)
        {
            logger.LogInformation($"Received [CancelRequested] {message.MobileOrderId}");

            try
            {

            using (var tx = dataStore.BeginTransaction())
            {
                dataStore.AddCancel(new CancelOrder
                {
                    MobileOrderId = message.MobileOrderId,
                    Status = "New",
                    Type = "Cease"
                });
            }

            var result = await externalMobileTelecomsNetworkService.PostCancel(new ExternalMobileTelecomsNetworkOrder
            {
                Reference = message.MobileOrderId
            });

            if (!result)
            {
                logger.LogInformation($"Failed to PostCancel to externalMobileTelecomsNetworkService");
                return false;
            }

            using (var tx = dataStore.BeginTransaction())
            {
                

                //if (!result)
                //{
                //    tx.Rollback();
                //    return false;
                //}

                dataStore.Sent(message.MobileOrderId);
                Publish(message.MobileOrderId);
            }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return false;
            }

            return true;
        }

        private void Publish(Guid mobileGlobalId)
        {
            messagePublisher.PublishAsync(new CancelOrderSentMessage
            {
                MobileOrderId = mobileGlobalId
            });
        }
    }
}