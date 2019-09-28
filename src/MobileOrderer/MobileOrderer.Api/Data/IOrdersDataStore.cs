using DapperDataAccess;
using System.Collections.Generic;

namespace MobileOrderer.Api.Data
{
    public interface IOrdersDataStore
    {
        void Add(MobileOrder order);
        IEnumerable<MobileOrder> GetAll();
        ITransaction BeginTransaction();
        IEnumerable<MobileOrder> GetNewOrders();
        void SetToProcessing(MobileOrder order);
    }
}