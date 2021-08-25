using Dapper;
using DapperDataAccess;
using Microsoft.Data.SqlClient;
using Mobiles.Api.Domain;
using Xunit.Abstractions;

namespace Data.Tests.Mobiles.Api
{
    public class MobilesDataAccess
    {
        private readonly string connectionString;
        private readonly ITestOutputHelper output;

        public MobilesDataAccess(ITestOutputHelper output, string connectionString)
        {
            this.output = output;
            this.connectionString = connectionString;
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