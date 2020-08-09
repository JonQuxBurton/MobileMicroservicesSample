using DapperDataAccess;
using System;
using System.Collections.Generic;

namespace SimCards.EventHandlers.Data
{
    public interface ISimCardOrdersDataStore
    {
        void Add(SimCardOrder order);
        SimCardOrder GetExisting(Guid mobileId, Guid mobileOrderId);
        IEnumerable<SimCardOrder> GetSent();

        ITransaction BeginTransaction();
        void Sent(Guid mobileOrderId);
        void Complete(Guid mobileOrderId);
    }
}