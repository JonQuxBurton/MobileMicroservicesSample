using DapperDataAccess;
using SimCards.EventHandlers.Data;
using System;
using System.Collections.Generic;

namespace IntegrationTests
{
    public class InMemorySimCardOrdersDataStore : ISimCardOrdersDataStore
    {
        public void Add(SimCardOrder order)
        {
            throw new NotImplementedException();
        }

        public ITransaction BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public void Complete(Guid mobileOrderId)
        {
            throw new NotImplementedException();
        }

        public SimCardOrder GetExisting(Guid mobileOrderId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SimCardOrder> GetSent()
        {
            throw new NotImplementedException();
        }

        public void Sent(Guid mobileOrderId)
        {
            throw new NotImplementedException();
        }
    }
}