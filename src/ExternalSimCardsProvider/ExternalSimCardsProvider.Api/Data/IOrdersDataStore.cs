using DapperDataAccess;
using System;
using System.Collections.Generic;

namespace ExternalSimCardsProvider.Api.Data
{
    public interface IOrdersDataStore
    {
        void Add(Order order);
        void Complete(Order order);

        Order GetByReference(Guid globalId);
        ITransaction BeginTransaction();
        int GetMaxId();
        IEnumerable<Order> GetAll();
    }
}