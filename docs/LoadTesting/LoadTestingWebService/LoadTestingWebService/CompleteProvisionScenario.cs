using System.Collections.Generic;

namespace LoadTestingWebService
{
    public class CompleteProvisionScenario : Scenario
    {
        private readonly IDataGenerator dataGenerator;

        public CompleteProvisionScenario(int virtualUsers, int iterations, bool requiresData, bool requiresDataInDatabase, IDataGenerator dataGenerator) 
            : base(virtualUsers, iterations, requiresData, requiresDataInDatabase)
        {
            this.dataGenerator = dataGenerator;
        }

        public override Dictionary<string, string> GetData()
        {
            var customerId = dataGenerator.GetExistingCustomerId();
            var mobileId = dataGenerator.GetNextGuid().ToString();
            var mobileOrderId = dataGenerator.GetNextGuid().ToString();
            var phoneNumber = dataGenerator.GetNextPhoneNumber();
            var contactName = dataGenerator.GetNextContactName();

            return new Dictionary<string, string>
            {
                {"customerId", customerId},
                {"mobileId", mobileId},
                {"mobileOrderId", mobileOrderId},
                {"phoneNumber", phoneNumber},
                {"contactName", contactName}
            };
        }
    }
}