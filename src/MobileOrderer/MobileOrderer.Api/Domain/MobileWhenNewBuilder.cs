using MobileOrderer.Api.Resources;
using System;
using System.Collections.Generic;
using Utils.Enums;
using static MobileOrderer.Api.Domain.Mobile;

namespace MobileOrderer.Api.Domain
{
    public class MobileWhenNewBuilder : IMobileWhenNewBuilder
    {
        private readonly Guid globalId;
        private readonly Guid customerId;
        private Order inFlightOrder;
        private readonly List<Order> orderHistory = new List<Order>();
        private readonly EnumConverter enumConverter;
        private readonly State initialState = State.New;
        private readonly PhoneNumber phoneNumber;

        public MobileWhenNewBuilder(Guid globalId, Guid customerId, PhoneNumber phoneNumber)
        {
            this.globalId = globalId;
            this.customerId = customerId;
            this.enumConverter = new EnumConverter();
            this.phoneNumber = phoneNumber;
        }

        public MobileWhenNewBuilder AddInFlightOrder(OrderToAdd order, Guid globalId)
        {
            var newStateName = new EnumConverter().ToName<State>(State.New);
            var dataEntity = new OrderDataEntity()
            {
                GlobalId = globalId,
                Name = order.Name,
                ContactPhoneNumber = order.ContactPhoneNumber,
                State = newStateName,
                Type = this.enumConverter.ToName<Order.OrderType>(Order.OrderType.Provision)
            };
            inFlightOrder = new Order(dataEntity);

            return this;
        }

        public Mobile Build()
        {
            var state = enumConverter.ToName<State>(initialState);
            var mobileDataEntity = new MobileDataEntity() 
            {
                Id = 0,
                GlobalId = globalId,
                State = state,
                CustomerId = customerId,
                PhoneNumber = phoneNumber.ToString()
            };
            return new Mobile(mobileDataEntity, inFlightOrder, orderHistory);
        }
    }
}
