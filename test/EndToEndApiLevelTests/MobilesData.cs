using System;
using System.Data.SqlClient;
using System.Threading;
using Dapper;
using MobileOrderer.Api.Domain;

namespace EndToEndApiLevelTests
{
    public class MobilesData : Data
    {
        private string connectionString;

        public MobilesData(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public MobileDataEntity CreateMobile(Guid globalId, string state)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var sql = $"insert into MobileOrderer.Mobiles (GlobalId, State) values (@globalId, @state)";
                connection.Execute(sql, new { globalId, state });
            }

            return GetMobile(globalId);
        }

        public MobileDataEntity GetMobile(Guid globalId)
        {
            var sql = $"select * from MobileOrderer.Mobiles where GlobalId=@globalId";

            using (var conn = new SqlConnection(connectionString))
            {
                var dbRow = conn.QueryFirstOrDefault(sql, new { globalId = globalId.ToString() });

                if (dbRow == null)
                    return null;

                var mobileDataEntity = new MobileDataEntity
                {
                    Id = dbRow.Id,
                    GlobalId = dbRow.GlobalId,
                    State = dbRow.State
                };
                return mobileDataEntity;
            }
        }
        
        public OrderDataEntity TryGetMobileOrder(Guid mobileOrderGlobalId, string state, TimeSpan? delay = null)
        {
            if (delay.HasValue)
                Thread.Sleep(delay.Value);
            
            return TryGet(() => GetMobileOrder(mobileOrderGlobalId, state));
        }

        public OrderDataEntity GetMobileOrder(Guid globalId, string state)
        {
            var sql = $"select * from MobileOrderer.Orders where GlobalId=@globalId and State=@state";

            using (var conn = new SqlConnection(connectionString))
            {
                var dbRow = conn.QueryFirstOrDefault(sql, new { globalId, state });

                if (dbRow == null)
                    return null;

                var order = new OrderDataEntity
                {
                    Id = dbRow.Id,
                    GlobalId = dbRow.GlobalId,
                    Name = dbRow.Name,
                    ContactPhoneNumber = dbRow.ContactPhoneNumber,
                    State = dbRow.State
                };
                return order;
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
                    ContactPhoneNumber = dbRow.ContactPhoneNumber,
                    State = dbRow.State
                };
                return order;
            }
        }
        
        public OrderDataEntity GetMobileOrderByGlobalId(Guid gloablId)
        {
            var sql = $"select * from MobileOrderer.Orders where GlobalId=@gloablId";

            using (var conn = new SqlConnection(connectionString))
            {
                var dbRow = conn.QueryFirstOrDefault(sql, new { gloablId });

                if (dbRow == null)
                    return null;

                var order = new OrderDataEntity
                {
                    Id = dbRow.Id,
                    GlobalId = dbRow.GlobalId,
                    Name = dbRow.Name,
                    ContactPhoneNumber = dbRow.ContactPhoneNumber,
                    State = dbRow.State
                };
                return order;
            }
        }
    }
}
