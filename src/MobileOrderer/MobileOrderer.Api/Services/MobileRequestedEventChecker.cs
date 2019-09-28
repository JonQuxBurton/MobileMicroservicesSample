using MinimalEventBus.JustSaying;
using MobileOrderer.Api.Data;
using MobileOrderer.Api.Messages;

namespace MobileOrderer.Api.Services
{
    public class MobileRequestedEventChecker : IMobileRequestedEventChecker
    {
        private readonly IOrdersDataStore orderDataStore;
        private readonly IMessagePublisher messagePublisher;

        public MobileRequestedEventChecker(IOrdersDataStore orderDataStore, IMessagePublisher messagePublisher)
        {
            this.orderDataStore = orderDataStore;
            this.messagePublisher = messagePublisher;
        }

        public void Check()
        {
            var newOrders = orderDataStore.GetNewOrders();

            foreach (var newOrder in newOrders)
            {
                using (orderDataStore.BeginTransaction())
                {
                    Publish(newOrder);
                    orderDataStore.SetToProcessing(newOrder);
                }
            }
        }

        private void Publish(MobileOrder order)
        {
            messagePublisher.PublishAsync(new MobileRequestedMessage
            {
                MobileOrderId = order.GlobalId,
                Name = order.Name,
                ContactPhoneNumber = order.ContactPhoneNumber
            });
        }
    }
}
