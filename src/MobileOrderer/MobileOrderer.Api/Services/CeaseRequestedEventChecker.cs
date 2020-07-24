using MinimalEventBus.JustSaying;
using MobileOrderer.Api.Data;
using MobileOrderer.Api.Domain;
using MobileOrderer.Api.Messages;
using System;
using Utils.DomainDrivenDesign;

namespace MobileOrderer.Api.Services
{
    public class CeaseRequestedEventChecker : IMobileEventsChecker
    {
        private readonly IGetNewCeasesQuery getMobilesQuery;
        private readonly IRepository<Mobile> mobileRepository;
        private readonly IMessagePublisher messagePublisher;

        public CeaseRequestedEventChecker(
            IGetNewCeasesQuery getCeasesMobilesQuery,
            IRepository<Mobile> mobileRepository,
            IMessagePublisher messagePublisher
            )
        {
            this.mobileRepository = mobileRepository;
            this.messagePublisher = messagePublisher;
            this.getMobilesQuery = getCeasesMobilesQuery;
        }

        public void Check()
        {
            var mobiles = this.getMobilesQuery.Get();

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
            messagePublisher.PublishAsync(new CeaseRequestedMessage
            {
                MobileOrderId = order.GlobalId
            });
        }
    }
}
