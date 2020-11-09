using System;
using System.Collections.Generic;

namespace LoadTestingWebService
{
    public class ActivateMobileScenario : Scenario
    {
        private readonly IDataGenerator dataGenerator;

        public ActivateMobileScenario(ScenarioSettings scenarioSettings, IDataGenerator dataGenerator)
            : base(scenarioSettings)
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