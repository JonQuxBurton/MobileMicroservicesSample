using System;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using ExternalMobileTelecomsNetwork.Api.Data;

namespace EndToEndApiLevelTests.Data
{
    public class ExternalMobileTelecomsNetworkData : Data
    {
        private string connectionString;

        public ExternalMobileTelecomsNetworkData(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public Order TryGetOrder(Guid mobileOrderId)
        {
            return TryGet(() => GetOrder(mobileOrderId));
        }

        public Order GetOrder(Guid reference)
        {
            var sql = $"select * from ExternalMobileTelecomsNetwork.Orders where Reference=@reference";

            using (var conn = new SqlConnection(connectionString))
            {
                var dbOrders = conn.Query(sql, new { reference });
                var dbOrder = dbOrders.FirstOrDefault();

                Order order = null;

                if (dbOrder != null)
                {
                    order = new Order
                    {
                        Reference = dbOrder.Reference,
                        Status = dbOrder.Status,
                        Type = dbOrder.Type,
                        CreatedAt = dbOrder.CreatedAt,
                        UpdatedAt = dbOrder.UpdatedAt
                    };
                }

                return order;
            }
        }
    }
}
