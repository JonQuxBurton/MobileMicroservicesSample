using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Options;

namespace LoadTestingWebService.Data
{
    public class DataStore : IDataStore
    {
        private readonly TestDataSettings testDataSettings;

        public DataStore(IOptions<TestDataSettings> testDataSettingsOptions)
        {
            this.testDataSettings = testDataSettingsOptions.Value;
        }

        public int SetupData(string scenario, Dictionary<string, string> data)
        {
            using var connection = new SqlConnection(testDataSettings.ConnectionString);
            var procedure = $"[SetupDataFor{scenario}]";

            var parameters = new DynamicParameters();
            foreach (var dataKey in data.Keys)
            {
                parameters.Add($"@{dataKey}", data[dataKey]);
            }
            parameters.Add("@newMobileId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var result = connection.Query(procedure, parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@newMobileId");
        }
    }
}