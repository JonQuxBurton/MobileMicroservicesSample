using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using Dapper;
using DapperDataAccess;
using Microsoft.Extensions.Options;

namespace SimCards.EventHandlers.Data
{
    public class SimCardOrdersDataStore : ISimCardOrdersDataStore
    {
        private const string SchemaName = "SimCards";
        private const string OrdersTableName = "Orders";
        private readonly string connectionString;
        private DbConnection connection;
        private Transaction currentTransaction;

        public SimCardOrdersDataStore(IOptions<Config> config)
        {
            this.connectionString = config.Value.ConnectionString;
        }

        public ITransaction BeginTransaction()
        {
            connection = new SqlConnection(connectionString);
            currentTransaction = new Transaction(connection);
            return currentTransaction;
        }

        public void Add(SimCardOrder order)
        {
            var sql = $"insert into {SchemaName}.{OrdersTableName}(Name, MobileId, MobileOrderId, Status, PhoneNumber) values (@Name, @MobileId, @MobileOrderId, @Status, @PhoneNumber)";
            this.connection.Execute( sql, new { order.Name, order.MobileId, order.MobileOrderId, order.Status, order.PhoneNumber }, currentTransaction.Get());
        }

        public SimCardOrder GetExisting(Guid mobileId, Guid mobileOrderId)
        {
            var sql = $"select * from {SchemaName}.{OrdersTableName} where MobileId=@mobileId and MobileOrderId=@mobileOrderId";

            using var conn = new SqlConnection(connectionString);
            var dbOrder = conn.QueryFirstOrDefault(sql, new { mobileId = mobileId.ToString() , mobileOrderId = mobileOrderId.ToString() });

            if (dbOrder == null)
                return null;

            return new SimCardOrder
            {
                PhoneNumber = dbOrder.PhoneNumber,
                MobileId = dbOrder.MobileId,
                MobileOrderId = dbOrder.MobileOrderId,
                Name = dbOrder.Name,
                Status = dbOrder.Status,
                CreatedAt = dbOrder.CreatedAt,
                UpdatedAt = dbOrder.UpdatedAt
            };
        }

        public void Sent(Guid mobileOrderId)
        {
            var sql = $"update {SchemaName}.{OrdersTableName} set Status='Sent' where MobileOrderId=@MobileOrderId";
            this.connection.Execute(sql, new { mobileOrderId }, currentTransaction.Get());
        }
        
        public void Complete(Guid mobileOrderId)
        {
            var sql = $"update {SchemaName}.{OrdersTableName} set Status='Completed' where MobileOrderId=@MobileOrderId";
            this.connection.Execute(sql, new { mobileOrderId }, currentTransaction.Get());
        }

        public IEnumerable<SimCardOrder> GetSent()
        {
            var sql = $"select * from {SchemaName}.{OrdersTableName} where Status='Sent'";
            var orders = new List<SimCardOrder>();

            using (var conn = new SqlConnection(connectionString))
            {
                var dbOrders = conn.Query(sql);

                foreach (var dbOrder in dbOrders)
                {
                    orders.Add(new SimCardOrder
                    {
                        PhoneNumber = dbOrder.PhoneNumber,
                        MobileId = dbOrder.MobileId,
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