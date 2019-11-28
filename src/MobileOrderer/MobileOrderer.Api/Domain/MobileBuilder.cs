using MobileOrderer.Api.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MobileOrderer.Api.Domain
{
    public class MobileBuilder : IMobileBuilder
    {
        private readonly Mobile.State initialState;
        private readonly Guid globalId;
        private MobileOrder inFlightOrder;
        private List<MobileOrder> orderHistory = new List<MobileOrder>();

        public MobileBuilder(Mobile.State initialState, Guid globalId)
        {
            this.initialState = initialState;
            this.globalId = globalId;
        }

        public void AddInFlightOrder(MobileOrder order)
        {
            inFlightOrder = order;
        }

        public void AddInFlightOrder(MobileOrderToAdd order, Guid globalId)
        {
            inFlightOrder = new MobileOrder(globalId, order.Name, order.ContactPhoneNumber, "New");
        }

        public void AddOrderToHistory(MobileOrder order)
        {
            orderHistory.Add(order);
        }

        public Mobile Build()
        {
            return new Mobile(initialState, globalId, 0, inFlightOrder, orderHistory);
        }
    }
}
