using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using DapperDataAccess;
using ExternalMobileTelecomsNetwork.Api.Configuration;
using Microsoft.Extensions.Options;

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

        public IEnumerable<Order> GetAll()
        {
            var sql = $"select * from {SchemaName}.{OrdersTableName} order by CreatedAt desc";
            var orders = new List<Order>();

            using var conn = new SqlConnection(connectionString);
            var dbOrders = conn.Query(sql);
            foreach (var dbOrder in dbOrders)
                orders.Add(ConvertToOrder(dbOrder));

            return orders;
        }

        public Order GetByReference(Guid reference)
        {
            var sql = $"select * from {SchemaName}.{OrdersTableName} where Reference=@reference";

            using var conn = new SqlConnection(connectionString);
            var dbOrders = conn.Query(sql, new {reference});
            var dbOrder = dbOrders.FirstOrDefault();

            Order order = null;

            if (dbOrder != null)
                order = ConvertToOrder((dbOrder));

            return order;
        }

        public Order GetByPhoneNumber(string phoneNumber, string status)
        {
            var sql = $"select * from {SchemaName}.{OrdersTableName} where PhoneNumber=@phoneNumber and Status=@status";

            using var conn = new SqlConnection(connectionString);
            var dbOrders = conn.Query(sql, new {phoneNumber, status});
            var dbOrder = dbOrders.FirstOrDefault();

            Order order = null;

            if (dbOrder != null)
                order = ConvertToOrder(dbOrder);

            return order;
        }

        public void Add(Order order)
        {
            var status = "New";
            var sql =
                $"insert into {SchemaName}.{OrdersTableName}(Reference, Type, Status, PhoneNumber, ActivationCode) values (@Reference, @Type, @Status, @PhoneNumber, @ActivationCode)";
            connection.Execute(sql, new {order.Reference, order.Type, status, order.PhoneNumber, order.ActivationCode},
                currentTransaction.Get());
        }

        public void Complete(Guid reference)
        {
            var sql = $"update {SchemaName}.{OrdersTableName} set Status='Completed' where Reference=@Reference";
            connection.Execute(sql, new {reference}, currentTransaction.Get());
        }

        public void Reject(Guid reference, string reason)
        {
            var sql =
                $"update {SchemaName}.{OrdersTableName} set Status='Rejected', Reason=@Reason  where Reference=@Reference";
            connection.Execute(sql, new {reference, reason}, currentTransaction.Get());
        }

        public void Cease(string phoneNumber, Guid reference)
        {
            var type = "Cease";
            var status = "New";
            var sql =
                $"insert into {SchemaName}.{OrdersTableName}(Reference, Type, Status, PhoneNumber) values (@Reference, @Type, @Status, @PhoneNumber)";
            connection.Execute(sql, new {reference, type, status, phoneNumber}, currentTransaction.Get());
        }

        public bool InsertActivationCode(ActivationCode activationCode)
        {
            var sql = $"insert into {SchemaName}.ActivationCodes (PhoneNumber, Code) values (@PhoneNumber, @Code)";
            var rows = connection.Execute(sql, new {activationCode.PhoneNumber, activationCode.Code},
                currentTransaction.Get());
            return rows > 0;
        }

        public bool UpdateActivationCode(ActivationCode existing)
        {
            var sql =
                $"update {SchemaName}.ActivationCodes set Code=@code, UpdatedAt=@updatedAt where PhoneNumber=@phoneNumber";
            var rows = connection.Execute(sql,
                new {existing.Code, existing.UpdatedAt, phoneNumber = existing.PhoneNumber}, currentTransaction.Get());

            return rows > 0;
        }

        public ActivationCode GetActivationCode(string phoneNumber)
        {
            var sql = $"select * from {SchemaName}.ActivationCodes where PhoneNumber=@phoneNumber";
            var dbEntities = connection.Query(sql, new {phoneNumber}, currentTransaction.Get());
            var dbEntity = dbEntities.FirstOrDefault();

            ActivationCode entity = null;

            if (dbEntity != null)
                entity = new ActivationCode
                {
                    PhoneNumber = dbEntity.PhoneNumber,
                    CreatedAt = dbEntity.CreatedAt,
                    UpdatedAt = dbEntity.UpdatedAt,
                    Code = dbEntity.Code
                };

            return entity;
        }

        private static Order ConvertToOrder(dynamic dbOrder)
        {
            var order = new Order
            {
                PhoneNumber = dbOrder.PhoneNumber,
                Reference = dbOrder.Reference,
                Type = dbOrder.Type,
                Status = dbOrder.Status,
                ActivationCode = dbOrder.ActivationCode,
                CreatedAt = dbOrder.CreatedAt,
                UpdatedAt = dbOrder.UpdatedAt,
                Reason = dbOrder.Reason
            };
            return order;
        }
    }
}