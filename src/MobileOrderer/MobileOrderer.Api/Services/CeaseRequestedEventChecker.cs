using Amazon.Runtime.Internal.Util;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<CeaseRequestedEventChecker> logger;
        private readonly IGetNewCeasesQuery getMobilesQuery;
        private readonly IRepository<Mobile> mobileRepository;
        private readonly IMessagePublisher messagePublisher;

        public CeaseRequestedEventChecker(
            ILogger<CeaseRequestedEventChecker> logger,
            IGetNewCeasesQuery getCeasesMobilesQuery,
            IRepository<Mobile> mobileRepository,
            IMessagePublisher messagePublisher
            )
        {
            this.mobileRepository = mobileRepository;
            this.messagePublisher = messagePublisher;
            this.logger = logger;
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
            Publish(mobile, mobile.InFlightOrder);
            mobile.OrderProcessing();
            mobileRepository.Update(mobile);
        }

        private void Publish(Mobile mobile, Order order)
        {
            logger.LogInformation("Publishing event [{event}] - MobileOrderId={orderId}", typeof(CeaseRequestedMessage).Name, order.GlobalId);

            messagePublisher.PublishAsync(new CeaseRequestedMessage
            {
                PhoneNumber = mobile.PhoneNumber.ToString(),
                MobileId = mobile.GlobalId,
                MobileOrderId = order.GlobalId
            });
        }
    }
}
