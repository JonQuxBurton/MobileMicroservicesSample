using MinimalEventBus.JustSaying;
using MobileOrderer.Api.Data;
using MobileOrderer.Api.Domain;
using MobileOrderer.Api.Messages;
using System;
using Utils.DomainDrivenDesign;

namespace MobileOrderer.Api.Services
{
    public class ActivationRequestedEventChecker : IActivationRequestedEventChecker
    {
        private readonly IMessagePublisher messagePublisher;
        private readonly IRepository<Mobile> mobileRepository;
        private readonly IGetNewActivationsQuery getNewActivationsQuery;

        public ActivationRequestedEventChecker(IMessagePublisher messagePublisher,
            IRepository<Mobile> mobileRepository,
            IGetNewActivationsQuery getNewActivationsQuery
            )
        {
            this.messagePublisher = messagePublisher;
            this.mobileRepository = mobileRepository;
            this.getNewActivationsQuery = getNewActivationsQuery;
        }

        public void Check()
        {
            var newActivations = this.getNewActivationsQuery.GetNew();

            foreach (var mobile in newActivations)
            {
                Publish(mobile.GlobalId, mobile.InFlightOrder);
                mobile.OrderProcessing();
                mobileRepository.Update(mobile);
            }
        }

        private void Publish(Guid mobileGlobalId, Order order)
        {
            messagePublisher.PublishAsync(new ActivationRequestedMessage
            {
                MobileOrderId = order.GlobalId,
                Name = order.Name,
                ContactPhoneNumber = order.ContactPhoneNumber
            });
        }
    }
}
