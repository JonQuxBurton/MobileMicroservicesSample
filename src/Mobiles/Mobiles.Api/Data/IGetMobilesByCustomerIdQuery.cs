using Mobiles.Api.Domain;
using System;
using System.Collections.Generic;

namespace Mobiles.Api.Data
{
    public interface IGetMobilesByCustomerIdQuery
    {
        IEnumerable<Mobile> Get(Guid customerId);
    }
}