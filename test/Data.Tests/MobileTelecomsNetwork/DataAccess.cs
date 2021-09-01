using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using DapperDataAccess;
using Microsoft.Extensions.Options;
using MobileTelecomsNetwork.EventHandlers;
using MobileTelecomsNetwork.EventHandlers.Data;
using MobileTelecomsNetwork.EventHandlers.Domain;
using Utils.Enums;
using Xunit.Abstractions;

namespace Data.Tests.MobileTelecomsNetwork
{
    public class DataAccess
    {
        private readonly string connectionString;
        private readonly EnumConverter enumConverter;
        private readonly List<Order> insertedOrders = new List<Order>();
        private readonly ITestOutputHelper output;

        public DataAccess(ITestOutputHelper output, string connectionString)
        {
            this.output = output;
            this.connectionString = connectionString;
            enumConverter = new EnumConverter();
        }

        public Order GetOrderById(Guid mobileOrderId)
        {
            var sql = "select * from MobileTelecomsNetwork.Orders where MobileOrderId=@mobileOrderId";
            using var conn = new SqlConnection(connectionString);
            var dbOrder = conn.QueryFirstOrDefault(sql, new {mobileOrderId = mobileOrderId.ToString()});

            if (dbOrder == null)
                return null;

            var orderType = enumConverter.ToEnum<OrderType>(dbOrder.Type.ToString().Trim());
            var orderStatus = enumConverter.ToEnum<OrderStatus>(dbOrder.Status.ToString().Trim());

            return new Order
            {
                PhoneNumber = dbOrder.PhoneNumber,
                MobileId = dbOrder.MobileId,
                MobileOrderId = dbOrder.MobileOrderId,
                Name = dbOrder.Name,
                Type = orderType,
                Status = orderStatus,
                CreatedAt = dbOrder.CreatedAt,
                UpdatedAt = dbOrder.UpdatedAt,
                Attempts = dbOrder.Attempts
            };
        }

        public void InsertOrder(Order order)
        {
            var options = Options.Create(new Config
            {
                ConnectionString = connectionString
            });

            var sut = new DataStore(options, enumConverter);

            output.WriteLine($"Inserting MobileTelecomsNetwork.Order with MobileId={order.MobileId}");

            using var tx = sut.BeginTransaction();
            sut.Add(order);

            insertedOrders.Add(order);
        }

        public void DeleteOrder(Guid mobileOrderId)
        {
            var sql = "delete from MobileTelecomsNetwork.Orders where MobileOrderId=@mobileOrderId";

            var connection = new SqlConnection(connectionString);
            var currentTransaction = new Transaction(connection);

            output.WriteLine($"Deleting MobileTelecomsNetwork.Order with MobileId={mobileOrderId}");
            connection.Execute(sql, new {mobileOrderId}, currentTransaction.Get());

            currentTransaction.Get().Commit();
        }

        public void CleanUp()
        {
            if (!insertedOrders.Any())
                return;

            foreach (var insertedOrder in insertedOrders) 
                DeleteOrder(insertedOrder.MobileOrderId);

        }
    }
}