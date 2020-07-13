using DapperDataAccess;
using System;
using System.Collections.Generic;

namespace MobileTelecomsNetwork.EventHandlers.Data
{
    public interface IDataStore
    {
        void AddActivation(ActivationOrder order);
        ITransaction BeginTransaction();
        void Sent(Guid mobileOrderId);
        IEnumerable<ActivationOrder> GetSent();
        void Complete(Guid mobileOrderId);
        void AddCancel(CancelOrder order);
    }
}