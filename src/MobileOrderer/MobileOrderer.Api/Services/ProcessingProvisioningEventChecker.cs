using MinimalEventBus.JustSaying;
using MobileOrderer.Api.Data;
using MobileOrderer.Api.Domain;
using MobileOrderer.Api.Messages;
using Utils.DomainDrivenDesign;

namespace MobileOrderer.Api.Services
{
    public class ProcessingProvisioningEventChecker : IMobileEventsChecker
    {
        private readonly IGetProcessingProvisioningMobilesQuery getNewMobilesQuery;
        private readonly IRepository<Mobile> mobileRepository;
        private readonly IMessagePublisher messagePublisher;

        public ProcessingProvisioningEventChecker(
            IGetProcessingProvisioningMobilesQuery getProcessingProvisioningMobilesQuery,
            IRepository<Mobile> mobileRepository,
            IMessagePublisher messagePublisher
            )
        {
            this.getNewMobilesQuery = getProcessingProvisioningMobilesQuery;
            this.mobileRepository = mobileRepository;
            this.messagePublisher = messagePublisher;
        }

        public void Check()
        {
            var newMobiles = this.getNewMobilesQuery.Get();

            foreach (var newMobile in newMobiles)
            {
                Execute(newMobile);
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
            messagePublisher.PublishAsync(new MobileRequestedMessage
            {
                MobileOrderId = order.GlobalId,
                Name = order.Name,
                ContactPhoneNumber = order.ContactPhoneNumber
            });
        }
    }
}
