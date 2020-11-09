using System.Collections.Generic;

namespace LoadTestingWebService
{
    public class CompleteProvisionScenario : Scenario
    {
        private readonly IDataGenerator dataGenerator;

        public CompleteProvisionScenario(ScenarioSettings scenarioSettings, IDataGenerator dataGenerator) 
            : base(scenarioSettings)
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