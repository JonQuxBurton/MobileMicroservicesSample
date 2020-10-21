using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace LoadTestingSetupApp
{
    internal class DataStore
    {
        private readonly string connectionString;

        public DataStore(Config config)
        {
            connectionString = config.ConnectionString;
        }

        public void SetupDataForCompleteProvision(string customerId, string mobileId, string mobileOrderId,
            string phoneNumber, string contactName)
        {
            using var connection = new SqlConnection(connectionString);
            var procedure = "[SetupDataForCompleteProvision]";
            var values = new
            {
                customerId,
                mobileId,
                mobileOrderId,
                phoneNumber,
                contactName
            };
            var result = connection.Query(procedure, values, commandType: CommandType.StoredProcedure);
        }

        public void SetupDataForActivate(string customerId, string mobileId, string phoneNumber, string activationCode)
        {
            using var connection = new SqlConnection(connectionString);
            var procedure = "[SetupDataForActivate]";
            var values = new {customerId, mobileId, phoneNumber, activationCode};
            var result = connection.Query(procedure, values, commandType: CommandType.StoredProcedure);
        }

        public void SetupDataForCompleteActivate(string customerId, string mobileId, string mobileOrderId,
            string phoneNumber)
        {
            using var connection = new SqlConnection(connectionString);
            var procedure = "[SetupDataForCompleteActivate]";
            var values = new {customerId, mobileId, mobileOrderId, phoneNumber};
            var result = connection.Query(procedure, values, commandType: CommandType.StoredProcedure);
        }
    }
}