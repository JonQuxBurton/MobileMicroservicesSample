﻿using Amazon.Runtime.Internal.Util;
using Microsoft.Extensions.Logging;
using MinimalEventBus.JustSaying;
using Mobiles.Api.Data;
using Mobiles.Api.Domain;
using Mobiles.Api.Messages;
using System;
using System.Threading.Tasks;
using Utils.DomainDrivenDesign;

namespace Mobiles.Api.Services
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
            try
            {
                var mobiles = this.getMobilesQuery.Get();

                foreach (var mobile in mobiles)
                {
                    Execute(mobile).Wait();
                }
            }
            catch (Exception e)
            {
                logger.LogError("Error checking with [{checker}]: {exception} ", nameof(CeaseRequestedEventChecker), e);
            }
        }

        private async Task Execute(Mobile mobile)
        {
            await Publish(mobile, mobile.InProgressOrder);
            mobile.OrderProcessing();
            mobileRepository.Update(mobile);
        }

        private async Task<bool> Publish(Mobile mobile, Order order)
        {
            logger.LogInformation("Publishing event [{event}] - MobileOrderId={orderId}", typeof(CeaseRequestedMessage).Name, order.GlobalId);

            return await messagePublisher.PublishAsync(new CeaseRequestedMessage
            {
                PhoneNumber = mobile.PhoneNumber.ToString(),
                MobileId = mobile.GlobalId,
                MobileOrderId = order.GlobalId
            });
        }
    }
}
