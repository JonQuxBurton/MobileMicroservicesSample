using System;
using System.Collections.Generic;

namespace LoadTestingWebService
{
    public class ActivateMobileScenario : Scenario
    {
        private readonly IDataGenerator dataGenerator;

        public ActivateMobileScenario(int virtualUsers, int iterations, bool requiresData, bool requiresDataInDatabase,
            IDataGenerator dataGenerator)
            : base(virtualUsers, iterations, requiresData, requiresDataInDatabase)
        {
            this.dataGenerator = dataGenerator;
        }

        public override Dictionary<string, string> GetData()
        {
            var customerId = dataGenerator.GetExistingCustomerId();
            var mobileId = Guid.NewGuid().ToString();

            var phoneNumber = dataGenerator.GetNextPhoneNumber();
            var activationCode = dataGenerator.GetNextActivationCode();
            return new Dictionary<string, string>
            {
                {"customerId", customerId},
                {"phoneNumber", phoneNumber},
                {"mobileId", mobileId},
                {"activationCode", activationCode}
            };
        }
    }
}