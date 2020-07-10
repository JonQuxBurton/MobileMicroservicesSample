using DapperDataAccess;
using System;

namespace ExternalMobileTelecomsNetwork.Api.Data
{
    public interface IDataStore
    {
        void Add(Order order);
        ITransaction BeginTransaction();
        void Complete(Guid reference);
        Order GetByReference(Guid reference);
    }
}