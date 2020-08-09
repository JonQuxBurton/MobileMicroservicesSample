using Dapper;
using Dapper.Contrib.Extensions;
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

        public Order GetByReference(Guid mobileReference, string status="New")
        {
            var sql = $"select * from {SchemaName}.{OrdersTableName} where MobileReference=@mobileReference and Status=@status";

            using (var conn = new SqlConnection(connectionString))
            {
                var dbOrders = conn.Query(sql, new { mobileReference, status });
                var dbOrder = dbOrders.FirstOrDefault();

                Order order = null;

                if (dbOrder != null)
                {
                    order = new Order
                    {
                        MobileReference = dbOrder.MobileReference,
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
            var sql = $"insert into {SchemaName}.{OrdersTableName}(MobileReference, Type, Status, PhoneNumber) values (@MobileReference, @Type, @Status, @PhoneNumber)";
            connection.Execute(sql, new { order.MobileReference, type, status, order.PhoneNumber }, currentTransaction.Get());
        }

        public void Complete(Guid mobileReference)
        {
            var sql = $"update {SchemaName}.{OrdersTableName} set Status='Completed' where MobileReference=@MobileReference";
            connection.Execute(sql, new { mobileReference }, currentTransaction.Get());
        }

        public void Cease(Guid mobileReference, string phoneNumber)
        {
            var type = "Cease";
            var status = "New";
            var sql = $"insert into {SchemaName}.{OrdersTableName}(MobileReference, Type, Status, PhoneNumber) values (@MobileReference, @Type, @Status, @PhoneNumber)";
            connection.Execute(sql, new { mobileReference, type, status, phoneNumber }, currentTransaction.Get());
        }

        public bool InsertActivationCode(ActivationCode activationCode)
        {
            var sql = $"insert into {SchemaName}.ActivationCodes (PhoneNumber, Code) values (@PhoneNumber, @Code)";
            var rows = connection.Execute(sql, new { activationCode.PhoneNumber, activationCode.Code}, currentTransaction.Get());
            return rows > 0;
        }

        public bool UpdateActivationCode(ActivationCode existing)
        {
            return connection.Update(new ActivationCode
            {
                Id = existing.Id,
                PhoneNumber = existing.PhoneNumber,
                Code = existing.Code,
                UpdatedAt = DateTime.Now
            }, currentTransaction.Get());
        }

        public ActivationCode GetActivationCode(string phoneNumber)
        {
            var sql = $"select * from {SchemaName}.ActivationCodes where PhoneNumber=@phoneNumber";
            var dbEntities = connection.Query(sql, new { phoneNumber }, currentTransaction.Get());
            var dbEntity = dbEntities.FirstOrDefault();

            ActivationCode entity = null;

            if (dbEntity != null)
            {
                entity = new ActivationCode
                {
                    PhoneNumber = dbEntity.PhoneNumber,
                    CreatedAt = dbEntity.CreatedAt,
                    UpdatedAt = dbEntity.UpdatedAt,
                    Code = dbEntity.Code
                };
            }

            return entity;
        }
    }
}
