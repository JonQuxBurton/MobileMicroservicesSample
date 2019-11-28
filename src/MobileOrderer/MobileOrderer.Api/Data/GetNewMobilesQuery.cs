using Microsoft.Extensions.Options;
using MobileOrderer.Api.Configuration;
using MobileOrderer.Api.Domain;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;

namespace MobileOrderer.Api.Data
{
    public class GetNewMobilesQuery : IGetNewMobilesQuery
    {
        private const string SchemaName = "MobileOrderer";
        private const string TableName = "Mobiles";
        private const string MobileOrdersTableName = "Orders";
        private readonly string connectionString;

        public GetNewMobilesQuery(IOptions<Config> config)
        {
            this.connectionString = config.Value.ConnectionString;
        }

        public IEnumerable<Mobile> GetNew()
        {
            var sql = $"select *,  {SchemaName}.{MobileOrdersTableName}.Id as OrderId from {SchemaName}.{TableName} join {SchemaName}.{MobileOrdersTableName} on Mobiles.Id=Orders.MobileId and Mobiles.State='New'";

            var buildersDictionary = new Dictionary<Guid, MobileBuilder>();

            using (var conn = new SqlConnection(connectionString))
            {
                var dbMobilesAndOrdes = conn.Query(sql);

                foreach (var dbMobileAndOrder in dbMobilesAndOrdes)
                {
                    if (!buildersDictionary.ContainsKey(dbMobileAndOrder.GlobalId))
                    {
                        Enum.TryParse(dbMobileAndOrder.State.Trim(), out Domain.Mobile.State state);
                        buildersDictionary.Add(dbMobileAndOrder.GlobalId, new MobileBuilder(state, dbMobileAndOrder.GlobalId));
                    }

                    if (buildersDictionary.TryGetValue(dbMobileAndOrder.GlobalId, out MobileBuilder builder))
                    {
                        builder.AddInFlightOrder(new MobileOrder(dbMobileAndOrder.Id, 
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
