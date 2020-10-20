using System.Data;
using Dapper;
using System.Data.SqlClient;

namespace LoadTestingSetupApp
{
    class DataStore
    {
        private readonly string connectionString;

        public DataStore(Config config)
        {
            this.connectionString = config.ConnectionString;
        }

        public void SetupDataForCompleteProvision(string customerId, string mobileId, string mobileOrderId, string phoneNumber, string contactName)
        {
            using var connection = new SqlConnection(connectionString);
            var procedure = "[SetupDataForCompleteProvision]";
            var values = new { customerId = customerId, mobileId = mobileId, mobileOrderId= mobileOrderId, phoneNumber= phoneNumber, contactName= contactName };
            var result = connection.Query(procedure, values, commandType: CommandType.StoredProcedure);
        }

        public void SetupDataForActivate(string customerId, string mobileId, string phoneNumber, string activationCode)
        {
            using var connection = new SqlConnection(connectionString);
            var procedure = "[SetupDataForActivate]";
            var values = new { customerId = customerId, mobileId = mobileId, phoneNumber = phoneNumber , activationCode = activationCode };
            var result = connection.Query(procedure, values, commandType: CommandType.StoredProcedure);
        }

        public void SetupDataForCompleteActivate(string customerId, string mobileId, string mobileOrderId, string phoneNumber)
        {
            using var connection = new SqlConnection(connectionString);
            var procedure = "[SetupDataForCompleteActivate]";
            var values = new { customerId = customerId, mobileId = mobileId, mobileOrderId = mobileOrderId, phoneNumber = phoneNumber };
            var result = connection.Query(procedure, values, commandType: CommandType.StoredProcedure);
        }
    }
}
