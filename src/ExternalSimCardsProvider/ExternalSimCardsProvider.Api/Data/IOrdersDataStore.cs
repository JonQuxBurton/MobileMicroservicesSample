using DapperDataAccess;
using System;

namespace ExternalSimCardsProvider.Api.Data
{
    public interface IOrdersDataStore
    {
        void Add(Order order);
        void Complete(Order order);

        Order GetByMobileReference(Guid globalId);
        ITransaction BeginTransaction();
        int GetMaxId();
    }
}