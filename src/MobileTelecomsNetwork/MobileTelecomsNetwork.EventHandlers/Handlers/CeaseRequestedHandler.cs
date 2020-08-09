using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MinimalEventBus.JustSaying;
using MobileTelecomsNetwork.EventHandlers.Data;
using MobileTelecomsNetwork.EventHandlers.Domain;
using MobileTelecomsNetwork.EventHandlers.Messages;

namespace MobileTelecomsNetwork.EventHandlers.Handlers
{
    public class CeaseRequestedHandler : IHandlerAsync<CeaseRequestedMessage>
    {
        private readonly ILogger<CeaseRequestedHandler> logger;
        private readonly IDataStore dataStore;
        private readonly IExternalMobileTelecomsNetworkService externalMobileTelecomsNetworkService;
        private readonly IMessagePublisher messagePublisher;
        private readonly IMonitoring monitoring;

        public CeaseRequestedHandler(ILogger<CeaseRequestedHandler> logger,
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

        public async Task<bool> Handle(CeaseRequestedMessage receivedEvent)
        {
            var eventName = receivedEvent.GetType().Name;
            logger.LogInformation("Received event [{eventName}] with MobileOrderId={MobileOrderId}", eventName, receivedEvent.MobileOrderId);

            try
            {
                using (var tx = dataStore.BeginTransaction())
                {
                    dataStore.Add(new Order
                    {
                        PhoneNumber = receivedEvent.PhoneNumber,
                        MobileId = receivedEvent.MobileId,
                        MobileOrderId = receivedEvent.MobileOrderId,
                        Status = "New",
                        Type = "Cease"
                    });
                }

                using (var tx = dataStore.BeginTransaction())
                {
                    var result = await externalMobileTelecomsNetworkService.PostCease(new ExternalMobileTelecomsNetworkOrder
                    {
                        PhoneNumber = receivedEvent.PhoneNumber,
                        MobileReference = receivedEvent.MobileId
                    });

                    if (!result)
                    {
                        logger.LogInformation("Failed to PostCease to externalMobileTelecomsNetworkService");

                        tx.Rollback();
                        monitoring.CeaseOrderFailed();
                        return false;
                    }

                    dataStore.Sent(receivedEvent.MobileOrderId);
                    Publish(receivedEvent.MobileOrderId);
                    monitoring.CeaseOrderSent();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while processing {eventName}", eventName);
                return false;
            }

            return true;
        }

        private void Publish(Guid mobileGlobalId)
        {
            logger.LogInformation("Publishing event [{event}]", typeof(CeaseOrderSentMessage).Name);

            messagePublisher.PublishAsync(new CeaseOrderSentMessage
            {
                MobileOrderId = mobileGlobalId
            });
        }
    }
}
