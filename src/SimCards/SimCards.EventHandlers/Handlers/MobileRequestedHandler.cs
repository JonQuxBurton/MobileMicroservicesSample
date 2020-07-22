using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MinimalEventBus.JustSaying;
using SimCards.EventHandlers.Data;
using SimCards.EventHandlers.Messages;
using System;
using Prometheus;
using SimCards.EventHandlers.Domain;

namespace SimCards.EventHandlers.Handlers
{
    public class MobileRequestedHandler : IHandlerAsync<MobileRequestedMessage>
    {
        private readonly ILogger<MobileRequestedHandler> logger;
        private readonly ISimCardOrdersDataStore simCardOrdersDataStore;
        private readonly ISimCardWholesaleService simCardWholesaleService;
        private readonly IMessagePublisher messagePublisher;
        private readonly IMonitoring monitoring;

        public MobileRequestedHandler(ILogger<MobileRequestedHandler> logger, ISimCardOrdersDataStore simCardOrdersDataStore, ISimCardWholesaleService simCardWholesaleService,
            IMessagePublisher messagePublisher, IMonitoring monitoring)
        {
            this.logger = logger;
            this.simCardOrdersDataStore = simCardOrdersDataStore;
            this.simCardWholesaleService = simCardWholesaleService;
            this.messagePublisher = messagePublisher;
            this.monitoring = monitoring;
        }

        public async Task<bool> Handle(MobileRequestedMessage message)
        {
            this.logger.LogInformation($"Received [MobileRequested] {message.Name} {message.ContactPhoneNumber}");

            try
            {
                var existingOrder = simCardOrdersDataStore.GetExisting(message.MobileOrderId);

                if (existingOrder != null)
                {
                    return true;
                }

                using (var tx = simCardOrdersDataStore.BeginTransaction())
                {
                    simCardOrdersDataStore.Add(new SimCardOrder()
                    {
                        Name = message.Name,
                        MobileOrderId = message.MobileOrderId,
                        Status = "New"
                    });
                }

                using (var tx = simCardOrdersDataStore.BeginTransaction())
                {
                    var result = await simCardWholesaleService.PostOrder(new SimCardWholesalerOrder
                    {
                        Reference = message.MobileOrderId,
                        Name = message.Name
                    });

                    if (!result)
                    {
                        tx.Rollback();
                        monitoring.SimCardOrderFailed();
                        return false;
                    }

                    this.simCardOrdersDataStore.Sent(message.MobileOrderId);

                    this.Publish(message.MobileOrderId);
                    monitoring.SimCardOrderSent();
                }

                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return false;
            }
        }

        public void Publish(Guid mobileGlobalId)
        {
            messagePublisher.PublishAsync(new OrderSentMessage
            {
                MobileOrderId = mobileGlobalId
            });
        }
    }
}
