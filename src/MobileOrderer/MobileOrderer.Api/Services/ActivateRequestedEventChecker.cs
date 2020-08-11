using Microsoft.Extensions.Logging;
using MinimalEventBus.JustSaying;
using MobileOrderer.Api.Data;
using MobileOrderer.Api.Domain;
using MobileOrderer.Api.Messages;
using Utils.DomainDrivenDesign;

namespace MobileOrderer.Api.Services
{
    public class ActivateRequestedEventChecker : IMobileEventsChecker
    {
        private readonly ILogger<ActivateRequestedEventChecker> logger;
        private readonly IGetNewActivatesQuery getNewActivatesQuery;
        private readonly IRepository<Mobile> mobileRepository;
        private readonly IMessagePublisher messagePublisher;

        public ActivateRequestedEventChecker(
            ILogger<ActivateRequestedEventChecker> logger,
            IGetNewActivatesQuery getNewActivatesQuery,
            IRepository<Mobile> mobileRepository,
            IMessagePublisher messagePublisher
        )
        {
            this.logger = logger;
            this.getNewActivatesQuery = getNewActivatesQuery;
            this.mobileRepository = mobileRepository;
            this.messagePublisher = messagePublisher;
        }

        public void Check()
        {
            var mobiles = this.getNewActivatesQuery.Get();

            foreach (var mobile in mobiles)
            {
                Execute(mobile);
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
            logger.LogInformation("Publishing event [{event}] - MobileOrderId={orderId}" , typeof(ActivateRequestedMessage).Name, order.GlobalId);

            messagePublisher.PublishAsync(new ActivateRequestedMessage
            {
                MobileId = mobile.GlobalId,
                PhoneNumber = mobile.PhoneNumber.ToString(),
                MobileOrderId = order.GlobalId,
                ActivationCode = order.ActivationCode
            });
        }
    }
}
