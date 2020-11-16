using Mobiles.Api.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils.DateTimes;
using Utils.Enums;
using static Mobiles.Api.Domain.Mobile;

namespace Mobiles.Api.Domain
{
    public class MobileWhenNewBuilder : IMobileWhenNewBuilder
    {
        private readonly IDateTimeCreator dateTimeCreator;
        private readonly Guid globalId;
        private readonly Guid customerId;
        private readonly List<Order> orders = new List<Order>();
        private readonly EnumConverter enumConverter;
        private readonly MobileState initialMobileState = MobileState.New;
        private readonly PhoneNumber phoneNumber;

        public MobileWhenNewBuilder(IDateTimeCreator dateTimeCreator, Guid globalId, Guid customerId, PhoneNumber phoneNumber)
        {
            this.dateTimeCreator = dateTimeCreator;
            this.globalId = globalId;
            this.customerId = customerId;
            this.enumConverter = new EnumConverter();
            this.phoneNumber = phoneNumber;
        }

        public MobileWhenNewBuilder AddInProgressOrder(OrderToAdd order, Guid orderGlobalId)
        {
            var newStateName = new EnumConverter().ToName<MobileState>(MobileState.New);
            var dataEntity = new OrderDataEntity()
            {
                GlobalId = orderGlobalId,
                Name = order.Name,
                ContactPhoneNumber = order.ContactPhoneNumber,
                State = newStateName,
                Type = this.enumConverter.ToName<Order.OrderType>(Order.OrderType.Provision)
            };
            orders.Add(new Order(dataEntity));

            return this;
        }

        public Mobile Build()
        {
            var state = enumConverter.ToName<MobileState>(initialMobileState);
            var mobileDataEntity = new MobileDataEntity() 
            {
                Id = 0,
                GlobalId = globalId,
                State = state,
                CustomerId = customerId,
                PhoneNumber = phoneNumber.ToString(),
                Orders = orders.Select(x => x.GetDataEntity()).ToList()
            };
            return new Mobile(dateTimeCreator, mobileDataEntity);
        }
    }
}
