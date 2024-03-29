﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Mobiles.Api.Domain;
using Utils.DateTimes;

namespace Mobiles.Api.Data
{
    public class GetMobilesByCustomerIdQuery : IGetMobilesByCustomerIdQuery
    {
        private readonly IDateTimeCreator dateTimeCreator;
        private readonly MobilesContext mobilesContext;

        public GetMobilesByCustomerIdQuery(MobilesContext mobilesContext, IDateTimeCreator dateTimeCreator)
        {
            this.mobilesContext = mobilesContext;
            this.dateTimeCreator = dateTimeCreator;
        }

        public IEnumerable<Mobile> Get(Guid customerId)
        {
            var mobiles = new List<Mobile>();

            foreach (var mobileDataEntity in mobilesContext.Mobiles
                .Include(x => x.Orders)
                .Where(x => x.CustomerId == customerId))
            {
                mobiles.Add(new Mobile(dateTimeCreator, mobileDataEntity));
            }

            return mobiles;
        }
    }
}