using System;
using System.Threading.Tasks;
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
            try
            {
                var newMobiles = this.getNewMobilesQuery.Get();

                foreach (var newMobile in newMobiles)
                {
                    Execute(newMobile).Wait();
                }
            }
            catch (Exception e)
            {
                logger.LogError("Error checking with [{checker}]: {exception} ", nameof(ProcessingProvisionEventChecker), e);
            }
        }

        private async Task Execute(Mobile mobile)
        {
            await Publish(mobile, mobile.InFlightOrder);
            mobile.OrderProcessing();
            mobileRepository.Update(mobile);
        }

        private async Task Publish(Mobile mobile, Order order)
        {
            logger.LogInformation("Publishing event [{event}] - PhoneNumber={phoneNumber} MobileOrderId={orderId}", typeof(ProvisionRequestedMessage).Name, mobile.PhoneNumber, order.GlobalId);

            var publishResult = await messagePublisher.PublishAsync(new ProvisionRequestedMessage
            {
                PhoneNumber = mobile.PhoneNumber.ToString(),
                MobileId = mobile.GlobalId,
                MobileOrderId = order.GlobalId,
                Name = order.Name,
                ContactPhoneNumber = order.ContactPhoneNumber
            });

            if (!publishResult)
                logger.LogError("Error while publishing event [{event}] - PhoneNumber={phoneNumber} MobileOrderId={orderId}", nameof(ProvisionRequestedMessage), mobile.PhoneNumber, order.GlobalId);
        }
    }
}
