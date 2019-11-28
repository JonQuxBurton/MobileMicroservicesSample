using Microsoft.Extensions.Options;
using MobileOrderer.Api.Configuration;
using MobileOrderer.Api.Domain;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;
using Utils.Enums;

namespace MobileOrderer.Api.Data
{
    public class GetNewMobilesQuery : IGetNewMobilesQuery
    {
        private const string SchemaName = "MobileOrderer";
        private const string TableName = "Mobiles";
        private const string MobileOrdersTableName = "Orders";
        private readonly string connectionString;
        private readonly IEnumConverter enumConverter;

        public GetNewMobilesQuery(IOptions<Config> config, IEnumConverter enumConverter)
        {
            this.connectionString = config.Value.ConnectionString;
            this.enumConverter = enumConverter;
        }

        public IEnumerable<Mobile> GetNew()
        {
            var sql = $"select *, {SchemaName}.{MobileOrdersTableName}.Id as OrderId from {SchemaName}.{TableName} join {SchemaName}.{MobileOrdersTableName} on Mobiles.Id=Orders.MobileId and Mobiles.State='New'";

            var buildersDictionary = new Dictionary<Guid, MobileBuilder>();

            using (var conn = new SqlConnection(connectionString))
            {
                var dbMobilesAndOrdes = conn.Query(sql);

                foreach (var dbMobileAndOrder in dbMobilesAndOrdes)
                {
                    if (!buildersDictionary.ContainsKey(dbMobileAndOrder.GlobalId))
                    {
                        var state = enumConverter.ToEnum<Mobile.State>(dbMobileAndOrder.State);
                        buildersDictionary.Add(dbMobileAndOrder.GlobalId, new MobileBuilder(state, dbMobileAndOrder.GlobalId));
                    }

                    if (buildersDictionary.TryGetValue(dbMobileAndOrder.GlobalId, out MobileBuilder builder))
                    {
                        builder.AddInFlightOrder(new MobileOrder(dbMobileAndOrder.OrderId, 
                            dbMobileAndOrder.GlobalId,
                            dbMobileAndOrder.MobileId,
                            dbMobileAndOrder.Name,
                            dbMobileAndOrder.ContactPhoneNumber,
                            dbMobileAndOrder.Status,
                            dbMobileAndOrder.CreatedAt,
                            dbMobileAndOrder.UpdatedAt));
                    }
                }            
            }

            var mobiles = new List<Mobile>();
            buildersDictionary.Values.AsList().ForEach(x => {
                mobiles.Add(x.Build());
            });

            return mobiles;
        }
    }
}
