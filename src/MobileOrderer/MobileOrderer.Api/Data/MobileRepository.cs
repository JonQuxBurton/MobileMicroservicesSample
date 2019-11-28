using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Microsoft.Extensions.Options;
using MobileOrderer.Api.Configuration;
using MobileOrderer.Api.Domain;
using Utils.DomainDrivenDesign;
using Utils.Enums;

namespace MobileOrderer.Api.Data
{
    public class MobileRepository : IRepository<Mobile>
    {
        private const string SchemaName = "MobileOrderer";
        private const string MobilesTableName = "Mobiles";
        private const string OrdersTableName = "Orders";
        private readonly string connectionString;
        private readonly IEnumConverter enumConverter;

        public MobileRepository(IOptions<Config> config, IEnumConverter enumConverter)
        {
            this.connectionString = config.Value.ConnectionString;
            this.enumConverter = enumConverter;
        }

        public Mobile GetById(Guid globalId)
        {
            using var conn = new SqlConnection(connectionString);
            var sql = $"select top 1 * from {SchemaName}.{MobilesTableName} where GlobalId=@GlobalId";
            var dbRows = conn.Query(sql, new { GlobalId = globalId.ToString() });
            var dbRow = dbRows.FirstOrDefault();

            if (dbRow == null)
                return null;

            var ordersSql = $"select * from {SchemaName}.{OrdersTableName} where MobileId=@MobileId";
            var ordersDbRows = conn.Query(ordersSql, new { MobileId = dbRow.Id });

            var orderHistory = new List<MobileOrder>();
            MobileOrder inFlightOrder = null;

            foreach (var dbOrder in ordersDbRows)
            {
                orderHistory.Add(new MobileOrder(
                    dbOrder.Id,
                    dbOrder.GlobalId,
                    dbOrder.MobileId,
                    dbOrder.Name,
                    dbOrder.ContactPhoneNumber,
                    dbOrder.Status.Trim(),
                    dbOrder.CreatedAt,
                    dbOrder.UpdatedAt));

                inFlightOrder = orderHistory.FirstOrDefault(x => x.Status == "New" || x.Status == "Pending");
                if (inFlightOrder != null)
                    orderHistory.Remove(inFlightOrder);
            }

            var state = enumConverter.ToEnum<Mobile.State>(dbRow.State);

            return new Mobile(state, dbRow.GlobalId, dbRow.Id, inFlightOrder, orderHistory);
        }

        public void Save(Mobile aggregateRoot)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            using var transaction = connection.BeginTransaction();
            var current = this.GetById(aggregateRoot.GlobalId);
            if (current == null)
                Insert(connection, transaction, aggregateRoot);
            else
                Update(connection, transaction, aggregateRoot);

            transaction.Commit();
        }

        private void Insert(SqlConnection connection, DbTransaction transaction, Domain.Mobile mobile)
        {
            var stateString = enumConverter.ToName<Mobile.State>(mobile.CurrentState);
            var sql = $"insert into {SchemaName}.{MobilesTableName} (GlobalId, State) values (@GlobalId, @State); SELECT CAST(SCOPE_IDENTITY() as int)";
            var mobileId = connection.Query<int>(sql, new { mobile.GlobalId, State=stateString }, transaction);

            var order = mobile.InFlightOrder;
            if (order != null)
            {
                var orderSql = $"insert into {SchemaName}.{OrdersTableName} (GlobalId, Name, MobileId, ContactPhoneNumber, Status) values (@GlobalId, @Name, @MobileId, @ContactPhoneNumber, @Status)";
                connection.Execute(orderSql, new { order.GlobalId, order.Name, mobileId, order.ContactPhoneNumber, order.Status }, transaction);
            }
        }

        private void Update(SqlConnection connection, DbTransaction transaction, Domain.Mobile mobile)
        {
            var stateString = enumConverter.ToName<Mobile.State>(mobile.CurrentState);
            var sql = $"update {SchemaName}.{MobilesTableName} set State=@State, UpdatedAt=GETDATE() where GlobalId=@GlobalId";
            connection.Execute(sql, new { mobile.GlobalId, State=stateString }, transaction);

            var order = mobile.InFlightOrder;
            var orderSql = $"update {SchemaName}.{OrdersTableName} set Status=@Status, UpdatedAt=GETDATE() where Id=@Id";
            connection.Execute(orderSql, new { order.Id, order.Status }, transaction);
        }
    }
}