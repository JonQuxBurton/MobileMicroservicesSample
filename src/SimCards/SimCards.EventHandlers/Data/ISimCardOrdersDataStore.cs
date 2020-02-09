using DapperDataAccess;
using System;

namespace SimCards.EventHandlers.Data
{
    public interface ISimCardOrdersDataStore
    {
        void Add(SimCardOrder order);
        SimCardOrder GetExisting(Guid mobileOrderId);
        ITransaction BeginTransaction();
        void Sent(Guid mobileOrderId);
    }
}