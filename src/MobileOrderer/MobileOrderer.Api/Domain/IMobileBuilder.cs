using MobileOrderer.Api.Resources;
using System;

namespace MobileOrderer.Api.Domain
{
    public interface IMobileBuilder
    {
        void AddInFlightOrder(MobileOrder order);
        void AddInFlightOrder(MobileOrderToAdd order, Guid globalId);
        Mobile Build();
    }
}