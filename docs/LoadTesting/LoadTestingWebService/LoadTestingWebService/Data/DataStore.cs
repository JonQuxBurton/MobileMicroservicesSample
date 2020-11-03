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

        public int SetupDataForCompleteProvision(string customerId, string mobileId, string mobileOrderId,
            string phoneNumber, string contactName)
        {
            using var connection = new SqlConnection(testDataSettings.ConnectionString);
            var procedure = "[SetupDataForCompleteProvision]";

            var parameters = new DynamicParameters();
            parameters.Add("@customerId", customerId);
            parameters.Add("@mobileId", mobileId);
            parameters.Add("@mobileOrderId", mobileOrderId);
            parameters.Add("@phoneNumber", phoneNumber);
            parameters.Add("@contactName", contactName);
            parameters.Add("@newMobileId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var result = connection.Query(procedure, parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@newMobileId");
        }

        public int SetupDataForActivate(string customerId, string mobileId, string phoneNumber, string activationCode)
        {
            using var connection = new SqlConnection(testDataSettings.ConnectionString);
            var procedure = "[SetupDataForActivate]";

            var parameters = new DynamicParameters();
            parameters.Add("@customerId", customerId);
            parameters.Add("@mobileId", mobileId);
            parameters.Add("@phoneNumber", phoneNumber);
            parameters.Add("@activationCode", activationCode);
            parameters.Add("@newMobileId", dbType: DbType.Int32, direction: ParameterDirection.Output);
            var result = connection.Execute(procedure, parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@newMobileId");
        }

        public int SetupDataForCompleteActivate(string customerId, string mobileId, string mobileOrderId,
            string phoneNumber)
        {
            using var connection = new SqlConnection(testDataSettings.ConnectionString);
            var procedure = "[SetupDataForCompleteActivate]";

            var parameters = new DynamicParameters();
            parameters.Add("@customerId", customerId);
            parameters.Add("@mobileId", mobileId);
            parameters.Add("@phoneNumber", phoneNumber);
            parameters.Add("@mobileOrderId", mobileOrderId);
            parameters.Add("@newMobileId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var result = connection.Execute(procedure, parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@newMobileId");
        }
    }
}