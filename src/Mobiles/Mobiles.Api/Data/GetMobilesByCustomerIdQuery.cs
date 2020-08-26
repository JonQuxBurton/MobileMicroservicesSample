using Mobiles.Api.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mobiles.Api.Data
{
    public class GetMobilesByCustomerIdQuery : IGetMobilesByCustomerIdQuery
    {
        private readonly MobilesContext mobilesContext;

        public GetMobilesByCustomerIdQuery(MobilesContext mobilesContext)
        {
            this.mobilesContext = mobilesContext;
        }

        public IEnumerable<Mobile> Get(Guid customerId)
        {
            var mobiles = new List<Mobile>();

            foreach (var mobileDataEntity in mobilesContext.Mobiles.Where(x => x.CustomerId == customerId))
                mobiles.Add(new Mobile(mobileDataEntity, null));

            return mobiles;
        }
    }
}
