using Dapper;
using Dapper.Contrib.Extensions;
using DapperDataAccess;
using ExternalMobileTelecomsNetwork.Api.Configuration;
using ExternalMobileTelecomsNetwork.Api.Resources;
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
                        Type = dbOrder.Type,
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
            var type = "Provision";
            var status = "New";
            var sql = $"insert into {SchemaName}.{OrdersTableName}(Reference, Type, Status) values (@Reference, @Type, @Status)";
            connection.Execute(sql, new { order.Reference, type, status }, currentTransaction.Get());
        }

        public void Complete(Guid reference)
        {
            var sql = $"update {SchemaName}.{OrdersTableName} set Status='Completed' where Reference=@Reference";
            connection.Execute(sql, new { Reference = reference.ToString() }, currentTransaction.Get());
        }

        public void Cease(Guid reference)
        {
            var type = "Cease";
            var status = "New";
            var sql = $"insert into {SchemaName}.{OrdersTableName}(Reference, Type, Status) values (@Reference, @Type, @Status)";
            connection.Execute(sql, new { reference, type, status }, currentTransaction.Get());
        }

        public bool InsertActivationCode(ActivationCode activationCode)
        {
            var sql = $"insert into {SchemaName}.ActivationCodes (Reference, Code) values (@Reference, @Code)";
            var rows = connection.Execute(sql, new { activationCode.Reference, activationCode.Code}, currentTransaction.Get());
            return rows > 0;
        }

        public bool UpdateActivationCode(ActivationCode existing)
        {
            return connection.Update(new ActivationCode
            {
                Id = existing.Id,
                Reference = existing.Reference,
                Code = existing.Code,
                UpdatedAt = DateTime.Now
            }, currentTransaction.Get());
        }

        public ActivationCode GetActivationCode(Guid reference)
        {
            var sql = $"select * from {SchemaName}.ActivationCodes where Reference=@reference";
            var dbEntities = connection.Query(sql, new { reference }, currentTransaction.Get());
            var dbEntity = dbEntities.FirstOrDefault();

            ActivationCode entity = null;

            if (dbEntity != null)
            {
                entity = new ActivationCode
                {
                    Reference = dbEntity.Reference,
                    CreatedAt = dbEntity.CreatedAt,
                    UpdatedAt = dbEntity.UpdatedAt,
                    Code = dbEntity.Code
                };
            }

            return entity;
        }

    }
}
