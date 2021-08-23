using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using DapperDataAccess;
using ExternalSimCardsProvider.Api.Configuration;
using Microsoft.Extensions.Options;

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
            connectionString = config.Value.ConnectionString;
        }

        public ITransaction BeginTransaction()
        {
            connection = new SqlConnection(connectionString);
            currentTransaction = new Transaction(connection);
            return currentTransaction;
        }

        public IEnumerable<Order> GetAll()
        {
            var sql = $"select * from {SchemaName}.{OrdersTableName} order by CreatedAt desc";

            var orders = new List<Order>();

            using (var conn = new SqlConnection(connectionString))
            {
                var dbOrders = conn.Query(sql);

                foreach (var dbOrder in dbOrders)
                {
                    var order = ConvertToOrder(dbOrder);
                    orders.Add(order);
                }
            }

            return orders;
        }

        public Order GetByReference(Guid reference)
        {
            var sql = $"select * from {SchemaName}.{OrdersTableName} where Reference=@reference";

            using var conn = new SqlConnection(connectionString);
            var dbOrders = conn.Query(sql, new {reference});
            var dbOrder = dbOrders.FirstOrDefault();

            if (dbOrder == null)
                return null;

            return ConvertToOrder(dbOrder);
        }

        public int GetMaxId()
        {
            var sql = $"select max(id) from {SchemaName}.{OrdersTableName}";

            using var conn = new SqlConnection(connectionString);
            return conn.ExecuteScalar<int>(sql);
        }

        public void Add(Order order)
        {
            var sql =
                $"insert into {SchemaName}.{OrdersTableName}(PhoneNumber, Reference, Status) values (@PhoneNumber, @Reference, @Status)";
            connection.Execute(sql, new {order.PhoneNumber, order.Reference, order.Status},
                currentTransaction.Get());
        }

        public void Complete(Order order)
        {
            var sql =
                $"update {SchemaName}.{OrdersTableName} set Status='Completed', ActivationCode=@ActivationCode where Reference=@Reference";
            connection.Execute(sql, new {order.Reference, order.ActivationCode}, currentTransaction.Get());
        }

        private static Order ConvertToOrder(dynamic dbOrder)
        {
            return new Order
            {
                Id = dbOrder.Id,
                PhoneNumber = dbOrder.PhoneNumber,
                Reference = dbOrder.Reference,
                Status = dbOrder.Status,
                ActivationCode = dbOrder.ActivationCode,
                CreatedAt = dbOrder.CreatedAt,
                UpdatedAt = dbOrder.UpdatedAt
            };
        }
    }
}