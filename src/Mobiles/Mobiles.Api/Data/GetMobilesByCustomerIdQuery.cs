using Mobiles.Api.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils.DateTimes;

namespace Mobiles.Api.Data
{
    public class GetMobilesByCustomerIdQuery : IGetMobilesByCustomerIdQuery
    {
        private readonly MobilesContext mobilesContext;
        private readonly IDateTimeCreator dateTimeCreator;

        public GetMobilesByCustomerIdQuery(MobilesContext mobilesContext, IDateTimeCreator dateTimeCreator)
        {
            this.mobilesContext = mobilesContext;
            this.dateTimeCreator = dateTimeCreator;
        }

        public IEnumerable<Mobile> Get(Guid customerId)
        {
            var mobiles = new List<Mobile>();

            foreach (var mobileDataEntity in mobilesContext.Mobiles.Where(x => x.CustomerId == customerId))
                mobiles.Add(new Mobile(dateTimeCreator, mobileDataEntity));

            return mobiles;
        }
    }
}
