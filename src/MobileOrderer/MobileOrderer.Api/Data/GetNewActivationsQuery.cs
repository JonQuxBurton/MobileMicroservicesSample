﻿using MobileOrderer.Api.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Utils.Enums;
using static MobileOrderer.Api.Domain.Order;

namespace MobileOrderer.Api.Data
{
    public class GetNewActivationsQuery : IGetNewActivationsQuery
    {
        private readonly MobilesContext mobilesContext;
        private readonly IEnumConverter enumConverter;

        public GetNewActivationsQuery(MobilesContext mobilesContext, IEnumConverter enumConverter)
        {
            this.mobilesContext = mobilesContext;
            this.enumConverter = enumConverter;
        }

        public IEnumerable<Mobile> GetNew()
        {
            var newStateName = enumConverter.ToName<Mobile.State>(Mobile.State.New);
            var activateOrderType = enumConverter.ToName<OrderType>(OrderType.Activate);
            var mobilesDataEntities = this.mobilesContext.Mobiles
                .Include(x => x.Orders)//.Where(x => x.State == newStateName);
                .Where(x => x.Orders.Any(y => y.Type == activateOrderType && y.State == newStateName))
                .ToList();

            var buildersDictionary = new Dictionary<Guid, MobileBuilder>();
            var mobiles = new List<Mobile>();

            foreach (var mobileDataEntity in mobilesDataEntities)
            {
                var inFlightOrderDataEntity = mobileDataEntity.Orders.FirstOrDefault(x => x.State.Trim() == newStateName);
                var inFlightOrder = new Order(inFlightOrderDataEntity);
                if (inFlightOrder != null)
                {
                    var orderHistoryDataEntities = mobileDataEntity.Orders.Except(new[] { inFlightOrderDataEntity });
                    var orderHistory = orderHistoryDataEntities.Select(x => new Order(x));
                    mobiles.Add(new Mobile(mobileDataEntity, inFlightOrder, orderHistory));
                }
            }

            return mobiles;
        }
    }
}
