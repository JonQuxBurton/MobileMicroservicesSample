using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Mobiles.Api.Data;
using Mobiles.Api.Resources;
using Utils.DomainDrivenDesign;
using Utils.Guids;

namespace Mobiles.Api.Domain
{
    public class MobilesService : IMobilesService
    {
        private readonly ILogger<MobilesService> logger;
        private readonly IRepository<Mobile> mobileRepository;
        private readonly IGuidCreator guidCreator;
        private readonly IGetNextMobileIdQuery getNextMobileIdQuery;

        public MobilesService(ILogger<MobilesService> logger, 
            IRepository<Mobile> mobileRepository, 
            IGuidCreator guidCreator,
            IGetNextMobileIdQuery getNextMobileIdQuery)
        {
            this.logger = logger;
            this.mobileRepository = mobileRepository;
            this.guidCreator = guidCreator;
            this.getNextMobileIdQuery = getNextMobileIdQuery;
        }

        public IEnumerable<string> GetAvailablePhoneNumbers()
        {
            var nextId = getNextMobileIdQuery.Get().ToString().PadLeft(3, '0');
            return new[] { $"07{nextId}000{nextId}" };
        }

        public Mobile Activate(Guid id, ActivateRequest activateRequest)
        {
            var mobile = this.mobileRepository.GetById(id);

            if (mobile == null)
            {
                logger.LogWarning("Attempt to Activate an unknown Mobile - MobileId: {MobileId}", id);
                return null;
            }

            var newStateName = Order.State.New.ToString();
            var orderType = Order.OrderType.Activate.ToString();
            var dataEntity = new OrderDataEntity()
            {
                GlobalId = this.guidCreator.Create(),
                ActivationCode = activateRequest.ActivationCode,
                State = newStateName,
                Type = orderType
            };
            var inProgressOrder = new Order(dataEntity);

            mobile.Activate(inProgressOrder);
            mobileRepository.Update(mobile);

            return mobile;
        }

        public Mobile Cease(Guid id)
        {
            var mobile = this.mobileRepository.GetById(id);

            if (mobile == null)
            {
                logger.LogWarning("Attempt to Cease an unknown Mobile - MobileId: {MobileId}", id);
                return null;
            }

            var newStateName = Order.State.New.ToString();
            var orderType = Order.OrderType.Cease.ToString();
            var dataEntity = new OrderDataEntity()
            {
                GlobalId = this.guidCreator.Create(),
                State = newStateName,
                Type = orderType
            };

            var inProgressOrder = new Order(dataEntity);
            mobile.Cease(inProgressOrder);
            mobileRepository.Update(mobile);

            return mobile;
        }
    }
}
