using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MinimalEventBus.JustSaying;
using SimCards.EventHandlers.Data;
using SimCards.EventHandlers.Messages;
using System;
using SimCards.EventHandlers.Domain;

namespace SimCards.EventHandlers.Handlers
{
    public class ProvisionRequestedHandler : IHandlerAsync<ProvisionRequestedMessage>
    {
        private readonly ILogger<ProvisionRequestedHandler> logger;
        private readonly ISimCardOrdersDataStore simCardOrdersDataStore;
        private readonly IExternalSimCardsProviderService simCardWholesaleService;
        private readonly IMessagePublisher messagePublisher;
        private readonly IMonitoring monitoring;

        public ProvisionRequestedHandler(ILogger<ProvisionRequestedHandler> logger, ISimCardOrdersDataStore simCardOrdersDataStore, IExternalSimCardsProviderService simCardWholesaleService,
            IMessagePublisher messagePublisher, IMonitoring monitoring)
        {
            this.logger = logger;
            this.simCardOrdersDataStore = simCardOrdersDataStore;
            this.simCardWholesaleService = simCardWholesaleService;
            this.messagePublisher = messagePublisher;
            this.monitoring = monitoring;
        }

        public async Task<bool> Handle(ProvisionRequestedMessage receivedEvent)
        {
            var eventName = receivedEvent.GetType().Name;
            logger.LogInformation("Received event [{eventName}] with Name={Name}, ContactPhoneNumber={ContactPhoneNumber}", eventName, receivedEvent.Name, receivedEvent.ContactPhoneNumber);

            try
            {
                var existingOrder = simCardOrdersDataStore.GetExisting(receivedEvent.MobileOrderId);

                if (existingOrder != null)
                {
                    return true;
                }

                using (var tx = simCardOrdersDataStore.BeginTransaction())
                {
                    simCardOrdersDataStore.Add(new SimCardOrder()
                    {
                        Name = receivedEvent.Name,
                        MobileOrderId = receivedEvent.MobileOrderId,
                        Status = "New"
                    });
                }

                using (var tx = simCardOrdersDataStore.BeginTransaction())
                {
                    var result = await simCardWholesaleService.PostOrder(new ExternalSimCardOrder
                    {
                        Reference = receivedEvent.MobileOrderId,
                        Name = receivedEvent.Name
                    });

                    if (!result)
                    {
                        tx.Rollback();
                        monitoring.SimCardOrderFailed();
                        return false;
                    }

                    simCardOrdersDataStore.Sent(receivedEvent.MobileOrderId);

                    Publish(receivedEvent.MobileOrderId);
                    monitoring.SimCardOrderSent();
                }

                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while processing {messageName}", eventName);
                return false;
            }
        }

        public void Publish(Guid mobileGlobalId)
        {
            logger.LogInformation("Publishing event [{event}]", typeof(ProvisionOrderSentMessage).Name);

            messagePublisher.PublishAsync(new ProvisionOrderSentMessage
            {
                MobileOrderId = mobileGlobalId
            });
        }
    }
}
