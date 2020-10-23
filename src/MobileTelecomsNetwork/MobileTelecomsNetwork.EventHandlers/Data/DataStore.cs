using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Data.SqlClient;
using Dapper;
using DapperDataAccess;
using Microsoft.Extensions.Options;
using MobileTelecomsNetwork.EventHandlers.Domain;
using Utils.Enums;

namespace MobileTelecomsNetwork.EventHandlers.Data
{
    public class DataStore : IDataStore
    {
        private const string SchemaName = "MobileTelecomsNetwork";
        private const string OrdersTableName = "Orders";
        private readonly string connectionString;
        private readonly IEnumConverter enumConverter;
        private DbConnection connection;
        private Transaction currentTransaction;

        public DataStore(IOptions<Config> config, IEnumConverter enumConverter)
        {
            connectionString = config.Value.ConnectionString;
            this.enumConverter = enumConverter;
        }

        public ITransaction BeginTransaction()
        {
            connection = new SqlConnection(connectionString);
            currentTransaction = new Transaction(connection);
            return currentTransaction;
        }

        public void Add(Order order)
        {
            var sql = $"insert into {SchemaName}.{OrdersTableName}(Name, PhoneNumber, MobileId, MobileOrderId, Status, Type) values (@Name, @PhoneNumber, @MobileId, @MobileOrderId, @Status, @Type)";
            connection.Execute(sql, new { order.Name, order.PhoneNumber, order.MobileId, order.MobileOrderId, Status=order.Status.ToString(), Type=order.Type.ToString() }, currentTransaction.Get());
        }

        public void Sent(Guid mobileOrderId)
        {
            var sql = $"update {SchemaName}.{OrdersTableName} set Status='Sent' where MobileOrderId=@MobileOrderId";
            connection.Execute(sql, new { mobileOrderId }, currentTransaction.Get());
        }

        public void Complete(Guid mobileOrderId)
        {
            var sql = $"update {SchemaName}.{OrdersTableName} set Status='Completed' where MobileOrderId=@MobileOrderId";
            connection.Execute(sql, new { mobileOrderId }, currentTransaction.Get());
        }

        public IEnumerable<Order> GetSent()
        {
            var sql = $"select * from {SchemaName}.{OrdersTableName} where Status='Sent'";
            var orders = new List<Order>();

            using (var conn = new SqlConnection(connectionString))
            {
                var dbOrders = conn.Query(sql);

                foreach (var dbOrder in dbOrders)
                {
                    var orderType = enumConverter.ToEnum<OrderType>(dbOrder.Type.ToString().Trim());
                    var orderStatus = enumConverter.ToEnum<OrderStatus>(dbOrder.Status.ToString().Trim());

                    orders.Add(new Order
                    {
                        MobileId = dbOrder.MobileId,
                        MobileOrderId = dbOrder.MobileOrderId,
                        Name = dbOrder.Name,
                        Status = orderStatus,
                        Type = orderType,
                        CreatedAt = dbOrder.CreatedAt,
                        UpdatedAt = dbOrder.UpdatedAt
                    });
                }
            }

            return orders;
        }
    }
}