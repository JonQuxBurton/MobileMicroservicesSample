using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MobileOrderer.Api.Domain;
using Utils.DomainDrivenDesign;
using Utils.Enums;

namespace MobileOrderer.Api.Data
{
    public class MobileRepository : IRepository<Mobile>
    {
        private readonly MobilesContext mobilesContext;
        private readonly IEnumConverter enumConverter;

        public MobileRepository(MobilesContext mobilesContext, IEnumConverter enumConverter)
        {
            this.mobilesContext = mobilesContext;
            this.enumConverter = enumConverter;
        }

        public void Add(Mobile aggregateRoot)
        {
            var mobileDbEntity = aggregateRoot.GetDataEntity();

            if (aggregateRoot.InFlightOrder != null)
            {
                var order = aggregateRoot.InFlightOrder;
                var state = enumConverter.ToName<Order.State>(order.CurrentState);
                mobileDbEntity.Orders = new List<OrderDataEntity>
                {
                    new OrderDataEntity
                    {
                        GlobalId = order.GlobalId,
                        Name = order.Name,
                        ContactPhoneNumber = order.ContactPhoneNumber,
                        State = state
                    }
                };
            }

            mobilesContext.Mobiles.Add(mobileDbEntity);
            mobilesContext.SaveChanges();
        }

        public Mobile GetById(Guid globalId)
        {
            var mobileDbEntity = mobilesContext.Mobiles.Include(x => x.Orders).FirstOrDefault(x => x.GlobalId == globalId);

            if (mobileDbEntity == null)
                return null;

            var newStateName = enumConverter.ToName<Mobile.State>(Mobile.State.New);
            var inFlightOrderDataEntity = mobileDbEntity.Orders.FirstOrDefault(x => x.State == newStateName);
            var inFlightOrder = new Order(inFlightOrderDataEntity);

            var orderHistoryDataEntities = mobileDbEntity.Orders.Except(new[] { inFlightOrderDataEntity });
            var orderHistory = new List<Order>();
            orderHistoryDataEntities.ToList().ForEach(x => orderHistory.Add(new Order(x)));

            return new Mobile(mobileDbEntity, inFlightOrder, orderHistory);
        }

        public void Update(Mobile aggregateRoot)
        {
            this.mobilesContext.SaveChanges();
        }
    }
}