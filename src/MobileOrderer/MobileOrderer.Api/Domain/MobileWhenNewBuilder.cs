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
        private Order inFlightOrder;
        private readonly List<Order> orderHistory = new List<Order>();
        private EnumConverter enumConverter;
        private State initialState = State.New;

        public MobileWhenNewBuilder(Guid globalId)
        {
            this.globalId = globalId;
            this.enumConverter = new EnumConverter();

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
            var mobileDataEntity = new MobileDataEntity() { Id = 0, GlobalId = globalId, State = state };
            return new Mobile(mobileDataEntity, inFlightOrder, orderHistory);
        }
    }
}
