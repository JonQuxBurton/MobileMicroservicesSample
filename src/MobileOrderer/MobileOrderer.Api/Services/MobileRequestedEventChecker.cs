using MinimalEventBus.JustSaying;
using MobileOrderer.Api.Data;
using MobileOrderer.Api.Domain;
using MobileOrderer.Api.Messages;
using System;
using Utils.DomainDrivenDesign;

namespace MobileOrderer.Api.Services
{
    public class MobileRequestedEventChecker : IMobileRequestedEventChecker
    {
        private readonly IMessagePublisher messagePublisher;
        private readonly IGetNewMobilesQuery getNewMobilesQuery;
        private readonly IRepository<Mobile> mobileRepository;

        public MobileRequestedEventChecker(IMessagePublisher messagePublisher,
            IGetNewMobilesQuery getNewMobilesQuery, IRepository<Mobile> mobileRepository
            )
        {
            this.messagePublisher = messagePublisher;
            this.getNewMobilesQuery = getNewMobilesQuery;
            this.mobileRepository = mobileRepository;
        }

        public void Check()
        {
            var newMobiles = this.getNewMobilesQuery.GetNew();

            foreach (var newMobile in newMobiles)
            {
                newMobile.Provision(newMobile.InFlightOrder);
                mobileRepository.Update(newMobile);

                Publish(newMobile.GlobalId, newMobile.InFlightOrder);
                newMobile.OrderProcessing();
                mobileRepository.Update(newMobile);
            }
        }

        private void Publish(Guid mobileGlobalId, Order order)
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
