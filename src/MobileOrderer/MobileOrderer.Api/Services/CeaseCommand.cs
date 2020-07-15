using MinimalEventBus.JustSaying;
using MobileOrderer.Api.Domain;
using MobileOrderer.Api.Messages;
using System;
using Utils.DomainDrivenDesign;

namespace MobileOrderer.Api.Services
{
    public class CeaseCommand : IMobileCommand
    {
        private readonly IRepository<Mobile> mobileRepository;
        private readonly IMessagePublisher messagePublisher;

        public CeaseCommand(IRepository<Mobile> mobileRepository, 
            IMessagePublisher messagePublisher)
        {
            this.mobileRepository = mobileRepository;
            this.messagePublisher = messagePublisher;
        }

        public void Execute(Mobile mobile)
        {
            Publish(mobile.GlobalId, mobile.InFlightOrder);
            mobile.OrderProcessing();
            mobileRepository.Update(mobile);
        }

        private void Publish(Guid mobileGlobalId, Order order)
        {
            messagePublisher.PublishAsync(new CeaseRequestedMessage
            {
                MobileOrderId = order.GlobalId
            });
        }
    }
}
