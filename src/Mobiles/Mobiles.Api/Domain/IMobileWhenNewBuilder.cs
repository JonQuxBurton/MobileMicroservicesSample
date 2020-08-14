using Mobiles.Api.Resources;
using System;

namespace Mobiles.Api.Domain
{
    public interface IMobileWhenNewBuilder
    {
        MobileWhenNewBuilder AddInFlightOrder(OrderToAdd order, Guid globalId);
        Mobile Build();
    }
}