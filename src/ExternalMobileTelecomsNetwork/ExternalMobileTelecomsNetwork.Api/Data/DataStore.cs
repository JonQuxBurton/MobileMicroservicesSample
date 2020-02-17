﻿using Dapper;
using DapperDataAccess;
using ExternalMobileTelecomsNetwork.Api.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

namespace ExternalMobileTelecomsNetwork.Api.Data
{
    public class DataStore : IDataStore
    {
        private const string SchemaName = "ExternalMobileTelecomsNetwork";
        private const string OrdersTableName = "Orders";
        private readonly string connectionString;
        private DbConnection connection;
        private ITransaction currentTransaction;

        public DataStore(IOptions<Config> config)
        {
            connectionString = config.Value.ConnectionString;
        }

        public ITransaction BeginTransaction()
        {
            connection = new SqlConnection(connectionString);
            currentTransaction = new Transaction(connection);
            return currentTransaction;
        }

        public Order GetByReference(Guid reference)
        {
            var sql = $"select * from {SchemaName}.{OrdersTableName} where Reference=@reference";

            using (var conn = new SqlConnection(connectionString))
            {
                var dbOrders = conn.Query(sql, new { reference });
                var dbOrder = dbOrders.FirstOrDefault();

                Order order = null;

                if (dbOrder != null)
                {
                    order = new Order
                    {
                        Reference = dbOrder.Reference,
                        Status = dbOrder.Status,
                        CreatedAt = dbOrder.CreatedAt,
                        UpdatedAt = dbOrder.UpdatedAt
                    };
                }

                return order;
            }
        }

        public void Add(Order order)
        {
            var sql = $"insert into {SchemaName}.{OrdersTableName}(Reference, Status) values (@Reference, @Status)";
            connection.Execute(sql, new { order.Reference, order.Status }, currentTransaction.Get());
        }

        public void Complete(Order order)
        {
            var sql = $"update {SchemaName}.{OrdersTableName} set Status='Completed' where Reference=@Reference";
            connection.Execute(sql, new { Reference = order.Reference.ToString() }, currentTransaction.Get());
        }
    }
}