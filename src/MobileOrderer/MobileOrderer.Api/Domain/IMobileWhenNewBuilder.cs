using MobileOrderer.Api.Resources;
using System;

namespace MobileOrderer.Api.Domain
{
    public interface IMobileWhenNewBuilder
    {
        MobileWhenNewBuilder AddInFlightOrder(OrderToAdd order, Guid globalId);
        Mobile Build();
    }
}