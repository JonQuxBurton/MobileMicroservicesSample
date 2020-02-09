using System;
using MobileOrderer.Api.Domain;

namespace MobileOrderer.Api.Data
{
    public interface IGetMobileByOrderIdQuery
    {
        Mobile Get(Guid orderId);
    }
}