using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Mobiles.Api.Data;
using Mobiles.Api.Domain;
using Utils.Enums;

namespace EndToEndApiLevelTests.DataAcess
{
    public class MobilesData : Retry
    {
        private readonly string connectionString;
        private readonly DbContextOptions<MobilesContext> contextOptions;

        public MobilesData(string connectionString)
        {
            this.connectionString = connectionString;
            var optionsBuilder = new DbContextOptionsBuilder<MobilesContext>();
            optionsBuilder.UseSqlServer(connectionString);
            contextOptions = optionsBuilder.Options;
        }

        public string GetNextPhoneNumber()
        {
            var prefix = "07001";
            using var mobilesContext = new MobilesContext(contextOptions);
            var sql = $"select PhoneNumber from Mobiles.Mobiles where PhoneNumber like '{prefix}%';";

            using var conn = new SqlConnection(connectionString);
            var dbRows = conn.Query(sql);
            var phoneNumbers = dbRows.Select(dbRow =>
            {
                var phoneNumberString = ((string) dbRow.PhoneNumber).Substring(prefix.Length);
                return int.Parse(phoneNumberString);
            }).ToList();
            var next = phoneNumbers.Max() + 1;

            return $"{prefix}{next.ToString().PadLeft(6, '0')}";
        }

        public Customer GetCustomerByGlobalId(Guid globalId)
        {
            using var mobilesContext = new MobilesContext(contextOptions);
            var sql = "select * from Mobiles.Customers where GlobalId=@globalId";

            using var conn = new SqlConnection(connectionString);
            var dbRow = conn.QueryFirstOrDefault(sql, new {globalId});

            if (dbRow == null)
                return null;

            var customer = new Customer
            {
                GlobalId = dbRow.GlobalId,
                Name = dbRow.Name
            };
            return customer;
        }

        public bool CreateMobile(PhoneNumber phoneNumber, Guid globalId, string state)
        {
            var dummyCustomerId = Guid.NewGuid();

            using (var connection = new SqlConnection(connectionString))
            {
                var sql =
                    "insert into Mobiles.Mobiles (PhoneNumber, GlobalId, State, CustomerId) values (@phoneNumber, @globalId, @state, @customerId)";
                connection.Execute(sql,
                    new {phoneNumber = phoneNumber.ToString(), globalId, state, customerId = dummyCustomerId});
            }

            return true;
        }

        public MobileDataEntity GetMobileByGlobalId(Guid globalId)
        {
            using var mobilesContext = new MobilesContext(contextOptions);
            var mobilesRepo = new MobileRepository(mobilesContext, new EnumConverter());

            return mobilesRepo.GetById(globalId).GetDataEntity();
        }

        public OrderDataEntity TryGetMobileOrderInState(Guid mobileOrderGlobalId, string state, TimeSpan? delay = null)
        {
            if (delay.HasValue)
                Thread.Sleep(delay.Value);

            return TryGet(() => GetMobileOrderInState(mobileOrderGlobalId, state));
        }

        private OrderDataEntity GetMobileOrderInState(Guid globalId, string state)
        {
            var sql = "select * from Mobiles.Orders where GlobalId=@globalId and State=@state";

            using var conn = new SqlConnection(connectionString);
            var dbRow = conn.QueryFirstOrDefault(sql, new {globalId, state});

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

        public OrderDataEntity GetMobileOrder(int mobileId)
        {
            var sql = "select * from Mobiles.Orders where MobileId=@mobileId";

            using var conn = new SqlConnection(connectionString);
            var dbRow = conn.QueryFirstOrDefault(sql, new {mobileId});

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

        public OrderDataEntity GetMobileOrderByGlobalId(Guid gloablId)
        {
            var sql = "select * from Mobiles.Orders where GlobalId=@gloablId";

            using var conn = new SqlConnection(connectionString);
            var dbRow = conn.QueryFirstOrDefault(sql, new {gloablId});

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