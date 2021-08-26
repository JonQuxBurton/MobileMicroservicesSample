using System.Collections.Generic;
using Dapper;
using DapperDataAccess;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Mobiles.Api.Data;
using Mobiles.Api.Domain;
using Utils.DateTimes;
using Utils.Enums;
using Xunit.Abstractions;

namespace Data.Tests.Mobiles.Api
{
    public class MobilesDataAccess
    {
        private readonly string connectionString;
        private readonly ITestOutputHelper output;
        private readonly List<Mobile> mobilesAdded;

        public MobilesDataAccess(ITestOutputHelper output, string connectionString)
        {
            this.output = output;
            this.connectionString = connectionString;
            mobilesAdded = new List<Mobile>();
        }

        public void Add(Mobile mobile)
        {
            var contextOptions = new DbContextOptionsBuilder<MobilesContext>()
                .UseSqlServer(ConfigurationData.ConnectionString)
                .Options;

            using var context = new MobilesContext(contextOptions);
            var mobilesRepository = new MobileRepository(context, new EnumConverter(), new DateTimeCreator());
            mobilesRepository.Add(mobile);
            mobilesAdded.Add(mobile);
        }

        public void Cleanup()
        {
            foreach (var mobile in mobilesAdded)
                Delete(mobile);
        }

        public void Delete(Mobile mobile)
        {
            var connection = new SqlConnection(connectionString);
            var currentTransaction = new Transaction(connection);
            output.WriteLine($"Deleting Mobiles.Mobiles with GlobalId={mobile.GlobalId}");

            var sql1 = "delete from Mobiles.Orders where MobileId=@mobileId";
            connection.Execute(sql1, new {mobileId = mobile.Id}, currentTransaction.Get());

            var sql2 = "delete from Mobiles.Mobiles where GlobalId=@globalId";
            connection.Execute(sql2, new {globalId = mobile.GlobalId}, currentTransaction.Get());

            currentTransaction.Get().Commit();
        }
    }
}