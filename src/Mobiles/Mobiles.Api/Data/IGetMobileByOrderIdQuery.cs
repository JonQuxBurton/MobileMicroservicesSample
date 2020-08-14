using System;
using Mobiles.Api.Domain;

namespace Mobiles.Api.Data
{
    public interface IGetMobileByOrderIdQuery
    {
        Mobile Get(Guid orderId);
    }
}