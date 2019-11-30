using MobileOrderer.Api.Resources;
using System;

namespace MobileOrderer.Api.Domain
{
    public interface IMobileBuilder
    {
        MobileBuilder AddInFlightOrder(OrderToAdd order, Guid globalId);
        MobileBuilder AddOrderToHistory(Order order);

        Mobile Build();
    }
}