using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Mobiles.Api.Domain;
using Utils.DateTimes;
using Utils.DomainDrivenDesign;
using Utils.Enums;

namespace Mobiles.Api.Data
{
    public class MobileRepository : IRepository<Mobile>
    {
        private readonly IDateTimeCreator dateTimeCreator;
        private readonly IEnumConverter enumConverter;
        private readonly MobilesContext mobilesContext;

        public MobileRepository(MobilesContext mobilesContext, IEnumConverter enumConverter,
            IDateTimeCreator dateTimeCreator)
        {
            this.mobilesContext = mobilesContext;
            this.enumConverter = enumConverter;
            this.dateTimeCreator = dateTimeCreator;
        }

        public void Add(Mobile aggregateRoot)
        {
            var mobileDbEntity = aggregateRoot.GetDataEntity();

            if (aggregateRoot.InProgressOrder != null)
            {
                var order = aggregateRoot.InProgressOrder;
                var state = enumConverter.ToName<Order.State>(order.CurrentState);
                var type = enumConverter.ToName<Order.OrderType>(order.Type);

                mobileDbEntity.Orders = new List<OrderDataEntity>
                {
                    new OrderDataEntity
                    {
                        GlobalId = order.GlobalId,
                        Name = order.Name,
                        ContactPhoneNumber = order.ContactPhoneNumber,
                        State = state,
                        Type = type
                    }
                };
            }

            mobilesContext.Mobiles.Add(mobileDbEntity);
            mobilesContext.SaveChanges();
        }

        public Mobile GetById(Guid globalId)
        {
            var mobileDbEntity = mobilesContext.Mobiles.Include(x => x.Orders)
                .FirstOrDefault(x => x.GlobalId == globalId);

            if (mobileDbEntity == null)
                return null;

            return new Mobile(dateTimeCreator, mobileDbEntity);
        }

        public void Update(Mobile aggregateRoot)
        {
            mobilesContext.SaveChanges();
        }
    }
}