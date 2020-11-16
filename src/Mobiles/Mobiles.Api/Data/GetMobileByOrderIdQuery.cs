﻿using Mobiles.Api.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Utils.DateTimes;
using Utils.Enums;

namespace Mobiles.Api.Data
{
    public class GetMobileByOrderIdQuery : IGetMobileByOrderIdQuery
    {
        private readonly MobilesContext mobilesContext;
        private readonly IEnumConverter enumConverter;
        private readonly IDateTimeCreator dateTimeCreator;

        public GetMobileByOrderIdQuery(MobilesContext mobilesContext, IEnumConverter enumConverter, IDateTimeCreator dateTimeCreator)
        {
            this.mobilesContext = mobilesContext;
            this.enumConverter = enumConverter;
            this.dateTimeCreator = dateTimeCreator;
        }

        public Mobile Get(Guid orderId)
        {
            var newStateName = enumConverter.ToName<Order.State>(Order.State.New);
            var processingStateName = enumConverter.ToName<Order.State>(Order.State.Processing);
            var sentStateName = enumConverter.ToName<Order.State>(Order.State.Sent);
            var mobileDataEntity = this.mobilesContext.Mobiles.Include(x => x.Orders)
                .Where(x => x.Orders.Any(y => y.GlobalId == orderId))
                .FirstOrDefault();

            if (mobileDataEntity == null)
                return null;

            Order inFlightOrder = null;
            IEnumerable<Order> orderHistory = null;

            var inFlightOrderDataEntity = mobileDataEntity.Orders.FirstOrDefault(x => x.State.Trim() == newStateName || x.State.Trim() == processingStateName || x.State.Trim() == sentStateName);
            if (inFlightOrderDataEntity != null)
            {
                inFlightOrder = new Order(inFlightOrderDataEntity);

                if (inFlightOrder != null)
                {
                    //var orderHistoryDataEntities = mobileDataEntity.Orders.Except(new[] { inFlightOrderDataEntity });
                    var orderHistoryDataEntities = mobileDataEntity.Orders;
                    orderHistory = orderHistoryDataEntities.Select(x => new Order(x));
                }
            }

            return new Mobile(dateTimeCreator, mobileDataEntity);
        }
    }
}
