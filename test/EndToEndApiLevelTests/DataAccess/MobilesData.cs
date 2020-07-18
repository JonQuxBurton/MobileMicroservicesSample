﻿using System;
using System.Data.SqlClient;
using System.Threading;
using Dapper;
using Microsoft.EntityFrameworkCore;
using MobileOrderer.Api.Data;
using MobileOrderer.Api.Domain;

namespace EndToEndApiLevelTests.DataAcess
{
    public class MobilesData : Retry
    {
        private string connectionString;
        private DbContextOptions<MobilesContext> contextOptions;

        public MobilesData(string connectionString)
        {
            this.connectionString = connectionString;
            var optionsBuilder = new DbContextOptionsBuilder<MobilesContext>();
            optionsBuilder.UseSqlServer(connectionString);
            contextOptions = optionsBuilder.Options;
        }

        public bool CreateMobile(Guid globalId, string state)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var sql = $"insert into MobileOrderer.Mobiles (GlobalId, State) values (@globalId, @state)";
                connection.Execute(sql, new { globalId, state });
            }

            return true;
        }

        public MobileDataEntity GetMobileByGlobalId(Guid globalId)
        {
            using (var mobilesContext = new MobilesContext(contextOptions)) 
            {
                var mobilesRepo = new MobileRepository(mobilesContext, new Utils.Enums.EnumConverter());

                return mobilesRepo.GetById(globalId).GetDataEntity();
            }
        }

        public OrderDataEntity TryGetMobileOrderInState(Guid mobileOrderGlobalId, string state, TimeSpan? delay = null)
        {
            if (delay.HasValue)
                Thread.Sleep(delay.Value);

            return TryGet(() => GetMobileOrderInState(mobileOrderGlobalId, state));
        }

        private  OrderDataEntity GetMobileOrderInState(Guid globalId, string state)
        {
            var sql = $"select * from MobileOrderer.Orders where GlobalId=@globalId and State=@state";

            using (var conn = new SqlConnection(connectionString))
            {
                var dbRow = conn.QueryFirstOrDefault(sql, new { globalId, state });

                if (dbRow == null)
                    return null;

                var order = new OrderDataEntity
                {
                    Id = dbRow.Id,
                    GlobalId = dbRow.GlobalId,
                    Name = dbRow.Name,
                    ContactPhoneNumber = dbRow.ContactPhoneNumber,
                    State = dbRow.State
                };
                return order;
            }
        }

        public OrderDataEntity GetMobileOrder(int mobileId)
        {
            var sql = $"select * from MobileOrderer.Orders where MobileId=@mobileId";

            using (var conn = new SqlConnection(connectionString))
            {
                var dbRow = conn.QueryFirstOrDefault(sql, new { mobileId });

                if (dbRow == null)
                    return null;

                var order = new OrderDataEntity
                {
                    Id = dbRow.Id,
                    GlobalId = dbRow.GlobalId,
                    Name = dbRow.Name,
                    ContactPhoneNumber = dbRow.ContactPhoneNumber,
                    State = dbRow.State
                };
                return order;
            }
        }

        public OrderDataEntity GetMobileOrderByGlobalId(Guid gloablId)
        {
            var sql = $"select * from MobileOrderer.Orders where GlobalId=@gloablId";

            using (var conn = new SqlConnection(connectionString))
            {
                var dbRow = conn.QueryFirstOrDefault(sql, new { gloablId });

                if (dbRow == null)
                    return null;

                var order = new OrderDataEntity
                {
                    Id = dbRow.Id,
                    GlobalId = dbRow.GlobalId,
                    Name = dbRow.Name,
                    ContactPhoneNumber = dbRow.ContactPhoneNumber,
                    State = dbRow.State
                };
                return order;
            }
        }
    }
}