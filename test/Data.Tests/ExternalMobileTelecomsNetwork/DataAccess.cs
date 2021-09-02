using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using DapperDataAccess;
using ExternalMobileTelecomsNetwork.Api.Configuration;
using ExternalMobileTelecomsNetwork.Api.Data;
using Microsoft.Extensions.Options;
using Xunit.Abstractions;

namespace Data.Tests.ExternalMobileTelecomsNetwork
{
    public class DataAccess
    {
        private readonly List<ActivationCode> activationCodes;
        private readonly string connectionString;
        private readonly List<Order> orders;
        private readonly ITestOutputHelper output;
        private DataStore dataStore;

        public DataAccess(ITestOutputHelper output, string connectionString)
        {
            this.output = output;
            this.connectionString = connectionString;
            orders = new List<Order>();
            activationCodes = new List<ActivationCode>();
        }

        public void InsertOrder(Order order)
        {
            var options = Options.Create(new Config
            {
                ConnectionString = connectionString
            });

            dataStore = new DataStore(options);

            output.WriteLine($"Inserting Order with Reference={order.Reference}");

            using var tx = dataStore.BeginTransaction();
            dataStore.Add(order);

            orders.Add(order);
        }

        public void CleanUp()
        {
            CleanUpOrders();
            CleanUpActivationCodes();
        }

        public void DeleteOrder(Order order)
        {
            if (order == null)
                return;

            var sql = "delete from ExternalMobileTelecomsNetwork.Orders where Reference=@reference";

            var connection = new SqlConnection(connectionString);
            var currentTransaction = new Transaction(connection);

            output.WriteLine($"Deleting ExternalMobileTelecomsNetwork.Order with Reference={order.Reference}");
            connection.Execute(sql, new {reference = order.Reference}, currentTransaction.Get());

            currentTransaction.Get().Commit();
        }

        public void DeleteActivationCode(ActivationCode activationCode)
        {
            if (activationCode == null)
                return;

            var sql = "delete from ExternalMobileTelecomsNetwork.ActivationCodes where PhoneNumber=@PhoneNumber";

            var connection = new SqlConnection(connectionString);
            var currentTransaction = new Transaction(connection);

            output.WriteLine(
                $"Deleting ExternalMobileTelecomsNetwork.ActivationCode with PhoneNumber={activationCode.PhoneNumber}");
            connection.Execute(sql, new {activationCode.PhoneNumber}, currentTransaction.Get());

            currentTransaction.Get().Commit();
        }

        public void InsertActivationCode(ActivationCode activationCode)
        {
            var options = Options.Create(new Config
            {
                ConnectionString = connectionString
            });

            dataStore = new DataStore(options);

            output.WriteLine($"Inserting ActivationCode for PhoneNumber={activationCode.PhoneNumber}");

            using var tx = dataStore.BeginTransaction();
            dataStore.InsertActivationCode(activationCode);

            activationCodes.Add(activationCode);
        }

        private void CleanUpOrders()
        {
            if (!orders.Any())
                return;

            var sql = "delete from ExternalMobileTelecomsNetwork.Orders where Reference=@Reference";

            var connection = new SqlConnection(connectionString);
            var currentTransaction = new Transaction(connection);

            foreach (var order in orders)
            {
                output.WriteLine($"Deleting ExternalMobileTelecomsNetwork.Order with Reference={order.Reference}");
                connection.Execute(sql, new {order.Reference}, currentTransaction.Get());
            }

            currentTransaction.Get().Commit();
        }

        private void CleanUpActivationCodes()
        {
            if (!activationCodes.Any())
                return;

            var sql = "delete from ExternalMobileTelecomsNetwork.ActivationCodes where PhoneNumber=@PhoneNumber";

            var connection = new SqlConnection(connectionString);
            var currentTransaction = new Transaction(connection);

            foreach (var activationCode in activationCodes)
            {
                output.WriteLine(
                    $"Deleting ExternalMobileTelecomsNetwork.ActivationCodes with PhoneNumber={activationCode.PhoneNumber}");
                connection.Execute(sql, new {activationCode.PhoneNumber}, currentTransaction.Get());
            }

            currentTransaction.Get().Commit();
        }
    }
}