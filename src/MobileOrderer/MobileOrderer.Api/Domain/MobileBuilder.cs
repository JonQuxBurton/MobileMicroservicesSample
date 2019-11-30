using MobileOrderer.Api.Resources;
using System;
using System.Collections.Generic;
using Utils.Enums;
using static MobileOrderer.Api.Domain.Mobile;

namespace MobileOrderer.Api.Domain
{
    public class MobileBuilder : IMobileBuilder
    {
        private readonly Guid globalId;
        private Order inFlightOrder;
        private readonly List<Order> orderHistory = new List<Order>();

        public MobileBuilder(Guid globalId)
        {
            this.globalId = globalId;
        }
        public MobileBuilder AddInFlightOrder(OrderToAdd order, Guid globalId)
        {
            var newStateName = new EnumConverter().ToName<State>(State.New);
            var dataEntity = new OrderDataEntity()
            { 
                GlobalId = globalId, 
                Name = order.Name,
                ContactPhoneNumber = order.ContactPhoneNumber,
                State = newStateName
            };
            inFlightOrder = new Order(dataEntity);

            return this;
        }

        public MobileBuilder AddOrderToHistory(Order order)
        {
            orderHistory.Add(order);
            return this;
        }

        public Mobile Build()
        {
            var enumConverter = new EnumConverter();
            var initialState = enumConverter.ToName<State>(State.New);
            var mobileDataEntity = new MobileDataEntity() { Id = 0, GlobalId = globalId, State = initialState };
            return new Mobile(mobileDataEntity, inFlightOrder, orderHistory);
        }
    }
}
