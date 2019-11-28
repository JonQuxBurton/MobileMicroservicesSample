using MinimalEventBus.JustSaying;
using MobileOrderer.Api.Data;
using MobileOrderer.Api.Domain;
using MobileOrderer.Api.Messages;
using System;
using System.Linq;
using Utils.DomainDrivenDesign;

namespace MobileOrderer.Api.Services
{
    public class MobileRequestedEventChecker : IMobileRequestedEventChecker
    {
        private readonly IMessagePublisher messagePublisher;
        private readonly IGetNewMobilesQuery getNewMobilesQuery;
        private readonly IRepository<Mobile> mobileRepository;

        public MobileRequestedEventChecker(IMessagePublisher messagePublisher,
            IGetNewMobilesQuery getNewMobilesQuery, IRepository<Mobile> mobileRepository)
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
                newMobile.Provision();
                this.mobileRepository.Save(newMobile);
                if (newMobile.InFlightOrder != null)
                    Publish(newMobile.GlobalId, newMobile.InFlightOrder);
            }
        }

        private void Publish(Guid mobileGlobalId, MobileOrder order)
        {
            messagePublisher.PublishAsync(new MobileRequestedMessage
            {
                MobileOrderId = mobileGlobalId, 
                Name = order.Name,
                ContactPhoneNumber = order.ContactPhoneNumber
            });
        }
    }
}
