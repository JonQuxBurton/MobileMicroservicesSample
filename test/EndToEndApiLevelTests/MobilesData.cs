using System;
using System.Data.SqlClient;
using Dapper;
using MobileOrderer.Api.Domain;

namespace EndToEndApiLevelTests
{
    public class MobilesData
    {
        private string connectionString;

        public MobilesData(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public MobileDataEntity GetMobile(Guid mobileOrderId)
        {
            var sql = $"select * from MobileOrderer.Mobiles where GlobalId=@mobileOrderId";

            using (var conn = new SqlConnection(connectionString))
            {
                var dbOrder = conn.QueryFirstOrDefault(sql, new { mobileOrderId = mobileOrderId.ToString() });

                if (dbOrder == null)
                    return null;

                var mobileDataEntity = new MobileDataEntity
                {
                    Id = dbOrder.Id,
                    GlobalId = dbOrder.GlobalId
                };
                return mobileDataEntity;
            }
        }

        public OrderDataEntity GetMobileOrder(int mobileId)
        {
            var sql = $"select * from MobileOrderer.Orders where MobileId=@mobileId";

            using (var conn = new SqlConnection(connectionString))
            {
                var dbRow = conn.QueryFirstOrDefault(sql, new { mobileId });

                if (dbRow == null)
                    return null;

                var order = new OrderDataEntity
                {
                    Id = dbRow.Id,
                    GlobalId = dbRow.GlobalId,
                    Name = dbRow.Name,
                    ContactPhoneNumber = dbRow.ContactPhoneNumber
                };
                return order;
            }
        }
    }
}
