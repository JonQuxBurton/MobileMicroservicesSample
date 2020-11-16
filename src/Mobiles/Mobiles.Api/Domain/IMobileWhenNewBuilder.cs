using Mobiles.Api.Resources;
using System;

namespace Mobiles.Api.Domain
{
    public interface IMobileWhenNewBuilder
    {
        MobileWhenNewBuilder AddInProgressOrder(OrderToAdd order, Guid orderGlobalId);
        Mobile Build();
    }
}