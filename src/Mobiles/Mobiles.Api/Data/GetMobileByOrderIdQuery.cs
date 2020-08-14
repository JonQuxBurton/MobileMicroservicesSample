﻿using Mobiles.Api.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Utils.Enums;

namespace Mobiles.Api.Data
{
    public class GetMobileByOrderIdQuery : IGetMobileByOrderIdQuery
    {
        private readonly MobilesContext mobilesContext;
        private readonly IEnumConverter enumConverter;

        public GetMobileByOrderIdQuery(MobilesContext mobilesContext, IEnumConverter enumConverter)
        {
            this.mobilesContext = mobilesContext;
            this.enumConverter = enumConverter;
        }

        public Mobile Get(Guid orderId)
        {
            var newStateName = enumConverter.ToName<Order.State>(Order.State.New);
            var processingStateName = enumConverter.ToName<Order.State>(Order.State.Processing);
            var sentStateName = enumConverter.ToName<Order.State>(Order.State.Sent);
            var mobileDataEntity = this.mobilesContext.Mobiles.Include(x => x.Orders).Where(x => x.Orders.Any(y => y.GlobalId == orderId)).FirstOrDefault();

            var inFlightOrderDataEntity = mobileDataEntity.Orders.FirstOrDefault(x => x.State.Trim() == newStateName || x.State.Trim() == processingStateName || x.State.Trim() == sentStateName);
            var inFlightOrder = new Order(inFlightOrderDataEntity);

            IEnumerable<Order> orderHistory = null;
            if (inFlightOrder != null)
            {
                var orderHistoryDataEntities = mobileDataEntity.Orders.Except(new[] { inFlightOrderDataEntity });
                orderHistory = orderHistoryDataEntities.Select(x => new Order(x));

            }

            return new Mobile(mobileDataEntity, inFlightOrder, orderHistory);
        }
    }
}