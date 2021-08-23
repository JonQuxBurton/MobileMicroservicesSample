using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using DapperDataAccess;
using ExternalSimCardsProvider.Api.Configuration;
using ExternalSimCardsProvider.Api.Data;
using Microsoft.Extensions.Options;
using Xunit.Abstractions;

namespace Data.Tests.ExternalSimCardsProvider.Api
{
    public class DataAccess
    {
        private readonly string connectionString;
        private readonly List<Order> orders;
        private readonly ITestOutputHelper output;
        private OrdersDataStore sut;

        public DataAccess(ITestOutputHelper output, string connectionString)
        {
            this.output = output;
            this.connectionString = connectionString;
            orders = new List<Order>();
        }

        public void InsertOrder(Order order)
        {
            var options = Options.Create(new Config
            {
                ConnectionString = connectionString
            });

            sut = new OrdersDataStore(options);

            output.WriteLine($"Inserting Order with Reference={order.Reference}");

            using var tx = sut.BeginTransaction();
            sut.Add(order);

            orders.Add(order);
        }

        public void CleanUp()
        {
            if (!orders.Any())
                return;

            var sql = "delete from ExternalSimCardsProvider.Orders where Reference=@Reference";

            var connection = new SqlConnection(connectionString);
            var currentTransaction = new Transaction(connection);

            foreach (var order in orders)
            {
                output.WriteLine($"Deleting ExternalSimCardsProvider.Order with Reference={order.Reference}");
                connection.Execute(sql, new {order.Reference }, currentTransaction.Get());
            }

            currentTransaction.Get().Commit();
        }

        public void DeleteOrder(Guid reference)
        {
            var sql = "delete from ExternalSimCardsProvider.Orders where Reference=@reference";

            var connection = new SqlConnection(connectionString);
            var currentTransaction = new Transaction(connection);
            
            output.WriteLine($"Deleting ExternalSimCardsProvider.Order with Reference={reference}");
            connection.Execute(sql, new { reference }, currentTransaction.Get());

            currentTransaction.Get().Commit();
        }
    }
}