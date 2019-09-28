using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MinimalEventBus.JustSaying;
using SimCards.EventHandlers.Data;
using SimCards.EventHandlers.Services;
using SimCards.EventHandlers.Messages;

namespace SimCards.EventHandlers.Handlers
{
    public class MobileRequestedHandler : IHandlerAsync<MobileRequestedMessage>
    {
        private readonly ILogger<MobileRequestedHandler> logger;
        private readonly ISimCardOrdersDataStore simCardOrdersDataStore;
        private readonly ISimCardWholesaleService simCardWholesaleService;

        public MobileRequestedHandler(ILogger<MobileRequestedHandler> logger, ISimCardOrdersDataStore simCardOrdersDataStore, ISimCardWholesaleService simCardWholesaleService)
        {
            this.logger = logger;
            this.simCardOrdersDataStore = simCardOrdersDataStore;
            this.simCardWholesaleService = simCardWholesaleService;
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
            }

            return true;
        }
    }
}
