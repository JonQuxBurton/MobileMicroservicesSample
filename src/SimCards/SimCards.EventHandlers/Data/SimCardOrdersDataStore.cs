using System;
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
            this.connection = new SqlConnection(connectionString);
            this.currentTransaction = new Transaction(this.connection);
            return this.currentTransaction;
        }

        public void Add(SimCardOrder order)
        {
            var sql = $"insert into {SchemaName}.{OrdersTableName}(Name, MobileOrderId, Status) values (@Name, @MobileOrderId, @Status)";
            this.connection.Execute( sql, new { order.Name, order.MobileOrderId, order.Status }, currentTransaction.Get());
        }

        public SimCardOrder GetExisting(Guid mobileOrderId)
        {
            var sql = $"select * from {SchemaName}.{OrdersTableName} where MobileOrderId=@MobileOrderId";

            using (var conn = new SqlConnection(connectionString))
            {
                var dbOrder = conn.QueryFirstOrDefault(sql, new { MobileOrderId = mobileOrderId.ToString() });

                if (dbOrder == null)
                    return null;

                return new SimCardOrder
                {
                    MobileOrderId = dbOrder.MobileOrderId,
                    Name = dbOrder.Name,
                    Status = dbOrder.Status,
                    CreatedAt = dbOrder.CreatedAt,
                    UpdatedAt = dbOrder.UpdatedAt
                };
            }
        }

        public void Sent(Guid mobileOrderId)
        {
            var sql = $"update {SchemaName}.{OrdersTableName} set Status='Sent' where MobileOrderId=@MobileOrderId";
            this.connection.Execute(sql, new { mobileOrderId }, currentTransaction.Get());
        }
    }
}