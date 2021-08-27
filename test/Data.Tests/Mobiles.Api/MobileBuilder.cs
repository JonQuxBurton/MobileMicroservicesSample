using System;
using System.Collections.Generic;
using Mobiles.Api.Domain;
using Utils.DateTimes;

namespace Data.Tests.Mobiles.Api
{
    public class MobileBuilder
    {
        private int counter;
        private Mobile.MobileState mobileState = Mobile.MobileState.New;
        private Order.State orderState = Order.State.New;
        private Order.OrderType orderType = Order.OrderType.Provision;
        private Guid? customerId;

        public MobileBuilder WithMobileState(Mobile.MobileState withMobileState)
        {
            mobileState = withMobileState;
            return this;
        }

        public MobileBuilder WithOrderState(Order.State withOrderState)
        {
            orderState = withOrderState;
            return this;
        }

        public MobileBuilder WithOrderType(Order.OrderType withOrderType)
        {
            orderType = withOrderType;
            return this;
        }
        
        public MobileBuilder WithCustomerId(Guid withCustomerId)
        {
            customerId = withCustomerId;
            return this;
        }

        public Mobile Build()
        {
            counter++;

            customerId ??= Guid.NewGuid();

            return new Mobile(new DateTimeCreator(), new MobileDataEntity
            {
                GlobalId = Guid.NewGuid(),
                CustomerId = customerId.Value,
                State = mobileState.ToString(),
                PhoneNumber = $"070000000{counter}",
                Orders = new List<OrderDataEntity>
                {
                    new()
                    {
                        GlobalId = Guid.NewGuid(),
                        Name = $"Jon {counter}",
                        ContactPhoneNumber = $"080000000{counter}",
                        Type = orderType.ToString(),
                        State = orderState.ToString()
                    }
                }
            });
        }
        
        public Mobile BuildWithoutOrder()
        {
            counter++;

            customerId ??= Guid.NewGuid();

            return new Mobile(new DateTimeCreator(), new MobileDataEntity
            {
                GlobalId = Guid.NewGuid(),
                CustomerId = customerId.Value,
                State = mobileState.ToString(),
                PhoneNumber = $"070000000{counter}",
                Orders = new List<OrderDataEntity>()
            });
        }
    }
}