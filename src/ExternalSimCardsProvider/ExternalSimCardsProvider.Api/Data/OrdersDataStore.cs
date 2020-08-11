using Dapper;
using DapperDataAccess;
using Microsoft.Extensions.Options;
using ExternalSimCardsProvider.Api.Configuration;
using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

namespace ExternalSimCardsProvider.Api.Data
{
    public class OrdersDataStore : IOrdersDataStore
    {
        private const string SchemaName = "ExternalSimCardsProvider";
        private const string OrdersTableName = "Orders";
        private readonly string connectionString;
        private DbConnection connection;
        private ITransaction currentTransaction;

        public OrdersDataStore(IOptions<Config> config)
        {
            this.connectionString = config.Value.ConnectionString;
        }

        public ITransaction BeginTransaction()
        {
            this.connection = new SqlConnection(connectionString);
            this.currentTransaction = new Transaction(this.connection);
            return this.currentTransaction;
        }

        public Order GetByReference(Guid reference)
        {
            var sql = $"select * from {SchemaName}.{OrdersTableName} where Reference=@reference";

            using (var conn = new SqlConnection(connectionString))
            {
                var dbOrders = conn.Query(sql, new { reference });
                var dbOrder = dbOrders.FirstOrDefault();

                Order order = null;

                if (dbOrder != null)
                {
                    order = new Order
                    {
                        PhoneNumber = dbOrder.PhoneNumber,
                        Reference= dbOrder.Reference,
                        Status = dbOrder.Status,
                        ActivationCode = dbOrder.ActivationCode,
                        CreatedAt = dbOrder.CreatedAt,
                        UpdatedAt = dbOrder.UpdatedAt
                    };
                }

                return order;
            }
        }

        public int GetMaxId()
        {
            var sql = $"select max(id) from {SchemaName}.{OrdersTableName}";

            using var conn = new SqlConnection(connectionString);
            return conn.ExecuteScalar<int>(sql);
        }

        public void Add(Order order)
        {
            var sql = $"insert into {SchemaName}.{OrdersTableName}(PhoneNumber, Reference, Status) values (@PhoneNumber, @Reference, @Status)";
            connection.Execute(sql, new { order.PhoneNumber, order.Reference, order.Status }, this.currentTransaction.Get());
        }

        public void Complete(Order order)
        {
            var sql = $"update {SchemaName}.{OrdersTableName} set Status='Completed', ActivationCode=@ActivationCode where Reference=@Reference";
            connection.Execute(sql, new { order.Reference, order.ActivationCode }, this.currentTransaction.Get());
        }
    }
}
