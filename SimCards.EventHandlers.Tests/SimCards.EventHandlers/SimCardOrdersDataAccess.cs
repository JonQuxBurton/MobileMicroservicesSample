using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using DapperDataAccess;
using Microsoft.Extensions.Options;
using SimCards.EventHandlers;
using SimCards.EventHandlers.Data;
using Xunit.Abstractions;

namespace Data.Tests.SimCards.EventHandlers
{
    public class SimCardOrdersDataAccess
    {
        private readonly string connectionString;
        private readonly List<SimCardOrder> insertedSimCardOrders;
        private readonly ITestOutputHelper output;
        private SimCardOrdersDataStore sut;

        public SimCardOrdersDataAccess(ITestOutputHelper output, string connectionString)
        {
            this.output = output;
            this.connectionString = connectionString;
            insertedSimCardOrders = new List<SimCardOrder>();
        }

        public void InsertSimCardOrder(SimCardOrder simCardOrder)
        {
            var options = Options.Create(new Config
            {
                ConnectionString = connectionString
            });

            sut = new SimCardOrdersDataStore(options);

            output.WriteLine($"Inserting SimCards.Order with MobileId={simCardOrder.MobileId}");

            using var tx = sut.BeginTransaction();
            sut.Add(simCardOrder);

            insertedSimCardOrders.Add(simCardOrder);
        }
        
        public void DeleteSimCardOrder(Guid mobileId)
        {
            var sql = "delete from SimCards.Orders where MobileId=@mobileId";

            var connection = new SqlConnection(connectionString);
            var currentTransaction = new Transaction(connection);

            output.WriteLine($"Deleting SimCards.Order with MobileId={mobileId}");
            connection.Execute(sql, new {mobileId}, currentTransaction.Get());

            currentTransaction.Get().Commit();
        }
        
        public void CleanUp()
        {
            if (!insertedSimCardOrders.Any())
                return;

            var sql = "delete from SimCards.Orders where MobileId=@mobileId";

            var connection = new SqlConnection(connectionString);
            var currentTransaction = new Transaction(connection);

            foreach (var simCardOrder in insertedSimCardOrders)
            {
                output.WriteLine($"Deleting SimCards.Order with MobileId={simCardOrder.MobileId}");
                connection.Execute(sql, new {simCardOrder.MobileId}, currentTransaction.Get());
            }

            currentTransaction.Get().Commit();
        }
    }
}