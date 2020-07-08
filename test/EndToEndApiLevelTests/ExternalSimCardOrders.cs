using System;
using System.Data.SqlClient;
using Dapper;
using System.Linq;
using SimCardWholesaler.Api.Data;

namespace EndToEndApiLevelTests
{
    public class ExternalSimCardOrders : Data
    {
        private string connectionString;

        public ExternalSimCardOrders(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public Order TryGetExternalSimCardOrder(Guid reference)
        {
            return TryGet(() => GetNewExternalSimCardOrder(reference));
        }

        public Order GetNewExternalSimCardOrder(Guid reference)
        {
            var status = "New";
            var sql = $"select * from SimCardWholesaler.Orders where Reference=@reference and Status=@status";

            using (var conn = new SqlConnection(connectionString))
            {
                var dbOrders = conn.Query(sql, new { reference, status });
                var dbOrder = dbOrders.FirstOrDefault();

                Order order = null;

                if (dbOrder != null)
                {
                    order = new Order
                    {
                        Reference = dbOrder.Reference,
                        Status = dbOrder.Status,
                        CreatedAt = dbOrder.CreatedAt,
                        UpdatedAt = dbOrder.UpdatedAt
                    };
                }

                return order;
            }
        }
    }
}