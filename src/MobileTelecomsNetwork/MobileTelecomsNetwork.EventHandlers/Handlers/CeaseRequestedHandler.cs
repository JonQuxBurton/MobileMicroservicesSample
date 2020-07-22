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

        public async Task<bool> Handle(CeaseRequestedMessage message)
        {
            logger.LogInformation($"Received [CeaseRequested] {message.MobileOrderId}");

            try
            {

            using (var tx = dataStore.BeginTransaction())
            {
                dataStore.Add(new Order
                {
                    MobileOrderId = message.MobileOrderId,
                    Status = "New",
                    Type = "Cease"
                });
            }

            using (var tx = dataStore.BeginTransaction())
            {
                var result = await externalMobileTelecomsNetworkService.PostCease(new ExternalMobileTelecomsNetworkOrder
                {
                    Reference = message.MobileOrderId
                });

                if (!result)
                {
                    logger.LogInformation($"Failed to PostCease to externalMobileTelecomsNetworkService");

                    tx.Rollback();
                    monitoring.CeaseOrderFailed();
                    return false;
                }

                dataStore.Sent(message.MobileOrderId);
                Publish(message.MobileOrderId);
                monitoring.CeaseOrderSent();
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
            messagePublisher.PublishAsync(new CeaseOrderSentMessage
            {
                MobileOrderId = mobileGlobalId
            });
        }
    }
}
