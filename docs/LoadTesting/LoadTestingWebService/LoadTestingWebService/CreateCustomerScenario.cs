using System.Collections.Generic;

namespace LoadTestingWebService
{
    public class CreateCustomerScenario : Scenario
    {
        public CreateCustomerScenario(int virtualUsers, int iterations, bool requiresData, bool requiresDataInDatabase) 
            : base(virtualUsers, iterations, requiresData, requiresDataInDatabase)
        {
        }

        public override Dictionary<string, string> GetData()
        {
            return new Dictionary<string, string>(); }
    }
}