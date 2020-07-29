using MinimalEventBus.JustSaying;
using MobileOrderer.Api.Data;
using MobileOrderer.Api.Domain;
using MobileOrderer.Api.Messages;
using System;
using Utils.DomainDrivenDesign;

namespace MobileOrderer.Api.Services
{
    public class ActivateRequestedEventChecker : IMobileEventsChecker
    {
        private readonly IGetNewActivatesQuery getNewActivatesQuery;
        private readonly IRepository<Mobile> mobileRepository;
        private readonly IMessagePublisher messagePublisher;

        public ActivateRequestedEventChecker(
            IGetNewActivatesQuery getNewActivatesQuery,
            IRepository<Mobile> mobileRepository,
            IMessagePublisher messagePublisher
        )
        {
            this.getNewActivatesQuery = getNewActivatesQuery;
            this.mobileRepository = mobileRepository;
            this.messagePublisher = messagePublisher;
        }

        public void Check()
        {
            var mobiles = this.getNewActivatesQuery.Get();

            foreach (var mobile in mobiles)
            {
                Execute(mobile);
            }
        }

        private void Execute(Mobile mobile)
        {
            Publish(mobile.InFlightOrder);
            mobile.OrderProcessing();
            mobileRepository.Update(mobile);
        }

        private void Publish(Order order)
        {
            messagePublisher.PublishAsync(new ActivateRequestedMessage
            {
                MobileOrderId = order.GlobalId,
                Name = order.Name,
                ContactPhoneNumber = order.ContactPhoneNumber
            });
        }
    }
}
