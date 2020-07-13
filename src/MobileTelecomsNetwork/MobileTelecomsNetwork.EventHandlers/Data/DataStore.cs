using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using Dapper;
using DapperDataAccess;
using Microsoft.Extensions.Options;

namespace MobileTelecomsNetwork.EventHandlers.Data
{
    public class DataStore : IDataStore
    {
        private const string SchemaName = "MobileTelecomsNetwork";
        private const string OrdersTableName = "Orders";
        private readonly string connectionString;
        private DbConnection connection;
        private Transaction currentTransaction;

        public DataStore(IOptions<Config> config)
        {
            connectionString = config.Value.ConnectionString;
        }

        public ITransaction BeginTransaction()
        {
            connection = new SqlConnection(connectionString);
            currentTransaction = new Transaction(connection);
            return currentTransaction;
        }

        public void AddActivation(ActivationOrder order)
        {
            var sql = $"insert into {SchemaName}.{OrdersTableName}(Name, MobileOrderId, Status) values (@Name, @MobileOrderId, @Status)";
            connection.Execute(sql, new { order.Name, order.MobileOrderId, order.Status }, currentTransaction.Get());
        }

        public void AddCancel(CancelOrder order)
        {
            var sql = $"insert into {SchemaName}.{OrdersTableName}(Name, MobileOrderId, Status) values (@Name, @MobileOrderId, @Status)";
            connection.Execute(sql, new { order.Name, order.MobileOrderId, order.Status }, currentTransaction.Get());
        }

        public void Sent(Guid mobileOrderId)
        {
            var sql = $"update {SchemaName}.{OrdersTableName} set Status='Sent' where MobileOrderId=@MobileOrderId";
            connection.Execute(sql, new { mobileOrderId }, currentTransaction.Get());
        }

        public void Complete(Guid mobileOrderId)
        {
            var sql = $"update {SchemaName}.{OrdersTableName} set Status='Completed' where MobileOrderId=@MobileOrderId";
            connection.Execute(sql, new { mobileOrderId }, currentTransaction.Get());
        }

        public IEnumerable<ActivationOrder> GetSent()
        {
            var sql = $"select * from {SchemaName}.{OrdersTableName} where Status='Sent'";
            var orders = new List<ActivationOrder>();

            using (var conn = new SqlConnection(connectionString))
            {
                var dbOrders = conn.Query(sql);

                foreach (var dbOrder in dbOrders)
                {
                    orders.Add(new ActivationOrder
                    {
                        MobileOrderId = dbOrder.MobileOrderId,
                        Name = dbOrder.Name,
                        Status = dbOrder.Status,
                        CreatedAt = dbOrder.CreatedAt,
                        UpdatedAt = dbOrder.UpdatedAt
                    });
                }
            }

            return orders;
        }
    }
}