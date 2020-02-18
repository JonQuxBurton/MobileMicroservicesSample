using MinimalEventBus.JustSaying;
using MobileOrderer.Api.Domain;
using MobileOrderer.Api.Messages;
using System;
using Utils.DomainDrivenDesign;

namespace MobileOrderer.Api.Services
{
    public class ActivationCommand : IMobileCommand
    {
        private readonly IRepository<Mobile> mobileRepository;
        private readonly IMessagePublisher messagePublisher;

        public ActivationCommand(IRepository<Mobile> mobileRepository, 
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
            messagePublisher.PublishAsync(new ActivationRequestedMessage
            {
                MobileOrderId = order.GlobalId,
                Name = order.Name,
                ContactPhoneNumber = order.ContactPhoneNumber
            });
        }
    }
}
