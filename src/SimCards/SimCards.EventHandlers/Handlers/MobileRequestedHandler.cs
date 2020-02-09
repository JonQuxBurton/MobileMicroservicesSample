using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MinimalEventBus.JustSaying;
using SimCards.EventHandlers.Data;
using SimCards.EventHandlers.Services;
using SimCards.EventHandlers.Messages;
using System;

namespace SimCards.EventHandlers.Handlers
{
    public class MobileRequestedHandler : IHandlerAsync<MobileRequestedMessage>
    {
        private readonly ILogger<MobileRequestedHandler> logger;
        private readonly ISimCardOrdersDataStore simCardOrdersDataStore;
        private readonly ISimCardWholesaleService simCardWholesaleService;
        private readonly IMessagePublisher messagePublisher;

        public MobileRequestedHandler(ILogger<MobileRequestedHandler> logger, ISimCardOrdersDataStore simCardOrdersDataStore, ISimCardWholesaleService simCardWholesaleService,
            IMessagePublisher messagePublisher)
        {
            this.logger = logger;
            this.simCardOrdersDataStore = simCardOrdersDataStore;
            this.simCardWholesaleService = simCardWholesaleService;
            this.messagePublisher = messagePublisher;
        }

        public async Task<bool> Handle(MobileRequestedMessage message)
        {
            this.logger.LogInformation($"Received [MobileRequested] {message.Name} {message.ContactPhoneNumber}");

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
                    return false;
                }

                this.simCardOrdersDataStore.Sent(message.MobileOrderId);
                this.Publish(message.MobileOrderId);
            }

            return true;
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
