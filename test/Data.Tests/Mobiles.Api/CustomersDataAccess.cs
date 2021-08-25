using System;
using Dapper;
using DapperDataAccess;
using Microsoft.Data.SqlClient;
using Mobiles.Api.Domain;
using Xunit.Abstractions;

namespace Data.Tests.Mobiles.Api
{
    public class CustomersDataAccess
    {
        private readonly string connectionString;
        private readonly ITestOutputHelper output;

        public CustomersDataAccess(ITestOutputHelper output, string connectionString)
        {
            this.output = output;
            this.connectionString = connectionString;
        }

        public void Delete(Guid globalId)
        {
            var sql = "delete from Mobiles.Customers where GlobalId=@globalId";

            var connection = new SqlConnection(connectionString);
            var currentTransaction = new Transaction(connection);

            output.WriteLine($"Deleting Mobiles.Customers with GlobalId={globalId}");
            connection.Execute(sql, new {globalId}, currentTransaction.Get());

            currentTransaction.Get().Commit();
        }
    }
}