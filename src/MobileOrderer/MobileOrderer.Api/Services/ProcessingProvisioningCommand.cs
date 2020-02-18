using MinimalEventBus.JustSaying;
using MobileOrderer.Api.Domain;
using MobileOrderer.Api.Messages;
using Utils.DomainDrivenDesign;

namespace MobileOrderer.Api.Services
{
    public class ProcessingProvisioningCommand : IMobileCommand
    {
        private readonly IRepository<Mobile> mobileRepository;
        private readonly IMessagePublisher messagePublisher;

        public ProcessingProvisioningCommand(IRepository<Mobile> mobileRepository,
            IMessagePublisher messagePublisher)
        {
            this.mobileRepository = mobileRepository;
            this.messagePublisher = messagePublisher;
        }

        public void Execute(Mobile mobile)
        {
            Publish(mobile.InFlightOrder);
            mobile.OrderProcessing();
            mobileRepository.Update(mobile);
        }

        private void Publish(Order order)
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
