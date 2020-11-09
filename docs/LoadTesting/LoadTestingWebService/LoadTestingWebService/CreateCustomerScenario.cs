using System.Collections.Generic;

namespace LoadTestingWebService
{
    public class CreateCustomerScenario : Scenario
    {
        public CreateCustomerScenario(ScenarioSettings scenarioSettings) 
            : base(scenarioSettings)
        {
        }

        public override Dictionary<string, string> GetData()
        {
            return new Dictionary<string, string>(); }
    }
}