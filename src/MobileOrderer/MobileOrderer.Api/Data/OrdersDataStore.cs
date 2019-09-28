using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using Dapper;
using DapperDataAccess;
using Microsoft.Extensions.Options;
using MobileOrderer.Api.Configuration;

namespace MobileOrderer.Api.Data
{
    public class OrdersDataStore : IOrdersDataStore
    {
        private const string SchemaName = "MobileOrderer";
        private const string OrdersTableName = "Orders";
        private readonly string connectionString;
        private DbConnection connection;
        private ITransaction currentTransaction;

        public OrdersDataStore(IOptions<Config> config)
        {
            this.connectionString = config.Value.ConnectionString;
        }

        public ITransaction BeginTransaction()
        {
            this.connection = new SqlConnection(connectionString);
            this.currentTransaction = new Transaction(this.connection);
            return this.currentTransaction;
        }

        public IEnumerable<MobileOrder> GetAll()
        {
            var sql = $"select * from {SchemaName}.{OrdersTableName} ";

            using (var conn = new SqlConnection(connectionString))
            {
                var dbOrders = conn.Query(sql);
                
                var orders = new List<MobileOrder>();

                foreach (var dbOrder in dbOrders)
                {
                    orders.Add(new MobileOrder() {
                        Name = dbOrder.Name,
                        GlobalId = dbOrder.GlobalId,
                        ContactPhoneNumber = dbOrder.ContactPhoneNumber,
                        Status = dbOrder.Status,
                        CreatedAt = dbOrder.CreatedAt,
                        UpdatedAt = dbOrder.UpdatedAt
                    });
                }

                return orders;
            }
        }

        public void Add(MobileOrder order)
        {
            var sql = $"insert into {SchemaName}.{OrdersTableName}(Name, GlobalId, ContactPhoneNumber, Status) values (@Name, @GlobalId, @ContactPhoneNumber, @Status)";
            this.connection.Execute(sql, new { order.Name, order.GlobalId, order.ContactPhoneNumber, order.Status}, this.currentTransaction.Get());
        }

        public IEnumerable<MobileOrder> GetNewOrders()
        {
            var sql = $"select * from {SchemaName}.{OrdersTableName} where Status='New'";

            using (var conn = new SqlConnection(connectionString))
            {
                var dbOrders = conn.Query(sql);

                var orders = new List<MobileOrder>();

                foreach (var dbOrder in dbOrders)
                {
                    orders.Add(new MobileOrder() {
                        Name = dbOrder.Name,
                        GlobalId = dbOrder.GlobalId,
                        ContactPhoneNumber = dbOrder.ContactPhoneNumber,
                        Status = dbOrder.Status,
                        CreatedAt = dbOrder.CreatedAt,
                        UpdatedAt = dbOrder.UpdatedAt
                    });
                }

                return orders;
            }
        }

        public void SetToProcessing(MobileOrder order)
        {
            var sql = $"update {SchemaName}.{OrdersTableName} set Status='Processing', UpdatedAt=GETDATE() where GlobalId=@GlobalId";
            this.connection.Execute(sql, new { order.GlobalId }, this.currentTransaction.Get());
        }
    }
}