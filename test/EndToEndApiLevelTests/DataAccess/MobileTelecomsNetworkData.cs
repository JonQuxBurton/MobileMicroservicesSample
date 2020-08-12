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

                var orderStatus = Enum.Parse(typeof(OrderStatus), dbRow.Status.ToString().Trim());
                var orderType = Enum.Parse(typeof(OrderType), dbRow.Type.ToString().Trim());

                return new Order
                {
                    MobileOrderId = dbRow.MobileOrderId,
                    Name = dbRow.Name,
                    Status = orderStatus,
                    Type = orderType, 
                    CreatedAt = dbRow.CreatedAt,
                    UpdatedAt = dbRow.UpdatedAt
                };
            }
        }
    }
}
