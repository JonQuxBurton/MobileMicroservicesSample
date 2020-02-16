using DapperDataAccess;
using System;

namespace ExternalMobileTelecomsNetwork.Api.Data
{
    public interface IDataStore
    {
        void Add(Order order);
        ITransaction BeginTransaction();
        void Complete(Order order);
        Order GetByReference(Guid reference);
    }
}