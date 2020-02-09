using Dapper;
using DapperDataAccess;
using Microsoft.Extensions.Options;
using SimCardWholesaler.Api.Configuration;
using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

namespace SimCardWholesaler.Api.Data
{
    public class OrdersDataStore : IOrdersDataStore
    {
        private const string SchemaName = "SimCardWholesaler";
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
                        Reference= dbOrder.Reference,
                        Status = dbOrder.Status,
                        CreatedAt = dbOrder.CreatedAt,
                        UpdatedAt = dbOrder.UpdatedAt
                    };
                }

                return order;
            }
        }

        public void Add(Order order)
        {
            var sql = $"insert into {SchemaName}.{OrdersTableName}(Reference, Status) values (@Reference, @Status)";
            this.connection.Execute(sql, new { order.Reference, order.Status }, this.currentTransaction.Get());
        }

        public void Complete(Order order)
        {
            var sql = $"update {SchemaName}.{OrdersTableName} set Status='Completed' where Reference=@Reference";
            this.connection.Execute(sql, new { Reference = order.Reference.ToString() }, this.currentTransaction.Get());
        }
    }
}
