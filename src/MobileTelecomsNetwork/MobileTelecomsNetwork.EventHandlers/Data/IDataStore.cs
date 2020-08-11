using DapperDataAccess;
using MobileTelecomsNetwork.EventHandlers.Domain;
using System;
using System.Collections.Generic;

namespace MobileTelecomsNetwork.EventHandlers.Data
{
    public interface IDataStore
    {
        void Add(Order order);
        ITransaction BeginTransaction();
        void Sent(Guid mobileOrderId);
        IEnumerable<Order> GetSent();
        void Complete(Guid mobileOrderId);
    }
}