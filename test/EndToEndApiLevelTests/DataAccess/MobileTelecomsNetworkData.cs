using System;
using System.Data.SqlClient;
using Dapper;
using MobileTelecomsNetwork.EventHandlers.Domain;

namespace EndToEndApiLevelTests.DataAcess
{
    public class MobileTelecomsNetworkData : Retry
    {
        private string connectionString;

        public MobileTelecomsNetworkData(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public Order TryGetOrder(Guid mobileOrderId)
        {
            return TryGet(() => GetOrder(mobileOrderId));
        }

        public Order GetOrder(Guid mobileOrderId)
        {
            var sql = $"select * from MobileTelecomsNetwork.Orders where MobileOrderId=@MobileOrderId";

            using (var conn = new SqlConnection(connectionString))
            {
                var dbRow = conn.QueryFirstOrDefault(sql, new { MobileOrderId = mobileOrderId.ToString() });

                if (dbRow == null)
                    return null;

                return new Order
                {
                    MobileOrderId = dbRow.MobileOrderId,
                    Name = dbRow.Name,
                    Status = dbRow.Status,
                    CreatedAt = dbRow.CreatedAt,
                    UpdatedAt = dbRow.UpdatedAt
                };
            }
        }
    }
}
