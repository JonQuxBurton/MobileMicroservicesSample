﻿using Mobiles.Api.Domain;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Utils.Enums;

namespace Mobiles.Api.Data
{
    public class GetProcessingProvisionMobilesQuery : IGetProcessingProvisionMobilesQuery
    {
        private readonly MobilesContext mobilesContext;
        private readonly IEnumConverter enumConverter;

        public GetProcessingProvisionMobilesQuery(MobilesContext mobilesContext, IEnumConverter enumConverter)
        {
            this.mobilesContext = mobilesContext;
            this.enumConverter = enumConverter;
        }

        public IEnumerable<Mobile> Get()
        {
            var mobilesDataEntities = this.mobilesContext.Mobiles.Include(x => x.Orders).Where(x => x.State == Mobile.State.ProcessingProvision.ToString()).ToList();
            var mobiles = new List<Mobile>();

            foreach (var mobileDataEntity in mobilesDataEntities)
            {
                var newStateName = enumConverter.ToName<Mobile.State>(Order.State.New);

                var inFlightOrderDataEntity = mobileDataEntity.Orders.FirstOrDefault(x => x.State.Trim() == newStateName);
                if (inFlightOrderDataEntity != null)
                {
                    var inFlightOrder = new Order(inFlightOrderDataEntity);
                    var orderHistoryDataEntities = mobileDataEntity.Orders.Except(new[] { inFlightOrderDataEntity });
                    var orderHistory = orderHistoryDataEntities.Select(x => new Order(x));
                    mobiles.Add(new Mobile(mobileDataEntity, inFlightOrder, orderHistory));
                }
            }

            return mobiles;
        }
    }
}