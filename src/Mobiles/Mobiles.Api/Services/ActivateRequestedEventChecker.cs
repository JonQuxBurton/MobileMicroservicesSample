﻿using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MinimalEventBus.JustSaying;
using Mobiles.Api.Data;
using Mobiles.Api.Domain;
using Mobiles.Api.Messages;
using Utils.DomainDrivenDesign;

namespace Mobiles.Api.Services
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

        private async void Execute(Mobile mobile)
        {
            if (await Publish(mobile, mobile.InFlightOrder))
            {
                mobile.OrderProcessing();
                mobileRepository.Update(mobile);
            }
        }

        private async Task<bool> Publish(Mobile mobile, Order order)
        {
            logger.LogInformation("Publishing event [{event}] - MobileOrderId={orderId}" , nameof(ActivateRequestedMessage), order.GlobalId);

            var publishResult = await messagePublisher.PublishAsync(new ActivateRequestedMessage
            {
                MobileId = mobile.GlobalId,
                PhoneNumber = mobile.PhoneNumber.ToString(),
                MobileOrderId = order.GlobalId,
                ActivationCode = order.ActivationCode
            });

            if (!publishResult)
                logger.LogError("Error while publishing event [{event}] - MobileOrderId={orderId}", nameof(ActivateRequestedMessage), order.GlobalId);

            return publishResult;
        }
    }
}
