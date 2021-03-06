﻿using System.Collections.Generic;

namespace LoadTestingWebService
{
    public class CompleteActivateScenario : Scenario
    {
        private readonly IDataGenerator dataGenerator;

        public CompleteActivateScenario(ScenarioSettings scenarioSettings, IDataGenerator dataGenerator)
            : base(scenarioSettings)
        {
            this.dataGenerator = dataGenerator;
        }

        public override Dictionary<string, string> GetData()
        {
            var customerId = dataGenerator.GetExistingCustomerId();
            var mobileId = dataGenerator.GetNextGuid().ToString();
            var mobileOrderId = dataGenerator.GetNextGuid().ToString();
            var phoneNumber = dataGenerator.GetNextContactPhoneNumber();

            return new Dictionary<string, string>
            {
                {"customerId", customerId},
                {"phoneNumber", phoneNumber},
                {"mobileId", mobileId},
                {"mobileOrderId", mobileOrderId}
            };
        }
    }
}