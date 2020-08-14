using Microsoft.Extensions.Logging;
using MinimalEventBus.JustSaying;
using Mobiles.Api.Data;
using Mobiles.Api.Domain;
using Mobiles.Api.Messages;
using Utils.DomainDrivenDesign;

namespace Mobiles.Api.Services
{
    public class ProcessingProvisionEventChecker : IMobileEventsChecker
    {
        private readonly ILogger<ProcessingProvisionEventChecker> logger;
        private readonly IGetProcessingProvisionMobilesQuery getNewMobilesQuery;
        private readonly IRepository<Mobile> mobileRepository;
        private readonly IMessagePublisher messagePublisher;

        public ProcessingProvisionEventChecker(
            ILogger<ProcessingProvisionEventChecker> logger,
            IGetProcessingProvisionMobilesQuery getProcessingProvisionMobilesQuery,
            IRepository<Mobile> mobileRepository,
            IMessagePublisher messagePublisher
            )
        {
            this.logger = logger;
            this.getNewMobilesQuery = getProcessingProvisionMobilesQuery;
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
            Publish(mobile, mobile.InFlightOrder);
            mobile.OrderProcessing();
            mobileRepository.Update(mobile);
        }

        private void Publish(Mobile mobile, Order order)
        {
            logger.LogInformation("Publishing event [{event}] - PhoneNumber={phoneNumber} MobileOrderId={orderId}", typeof(ProvisionRequestedMessage).Name, mobile.PhoneNumber, order.GlobalId);

            messagePublisher.PublishAsync(new ProvisionRequestedMessage
            {
                PhoneNumber = mobile.PhoneNumber.ToString(),
                MobileId = mobile.GlobalId,
                MobileOrderId = order.GlobalId,
                Name = order.Name,
                ContactPhoneNumber = order.ContactPhoneNumber
            });
        }
    }
}
