using System;
using System.Data.SqlClient;
using Dapper;
using SimCards.EventHandlers.Data;

namespace EndToEndApiLevelTests
{
    public class SimCardsData : Data
    {
        private string connectionString;

        public SimCardsData(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public SimCardOrder TryGetSimCardOrder(Guid mobileOrderId)
        {
            return TryGet(() => GetSimCardOrder(mobileOrderId));
        }

        public SimCardOrder GetSimCardOrder(Guid mobileOrderId)
        {
            var sql = $"select * from SimCards.Orders where MobileOrderId=@MobileOrderId";

            using (var conn = new SqlConnection(connectionString))
            {
                var dbRow = conn.QueryFirstOrDefault(sql, new { MobileOrderId = mobileOrderId.ToString() });

                if (dbRow == null)
                    return null;

                return new SimCardOrder
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
