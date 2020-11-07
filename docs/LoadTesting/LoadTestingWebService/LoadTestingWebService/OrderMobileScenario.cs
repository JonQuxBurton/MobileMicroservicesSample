using System.Collections.Generic;

namespace LoadTestingWebService
{
    public class OrderMobileScenario : Scenario
    {
        private readonly IDataGenerator dataGenerator;

        public OrderMobileScenario(int virtualUsers, int iterations, bool requiresData, bool requiresDataInDatabase, IDataGenerator dataGenerator) 
            : base(virtualUsers, iterations, requiresData, requiresDataInDatabase)
        {
            this.dataGenerator = dataGenerator;
        }

        public override Dictionary<string, string> GetData()
        {
            return new Dictionary<string, string>
            {
                {"customerId", dataGenerator.GetExistingCustomerId()},
                {"phoneNumber", dataGenerator.GetNextPhoneNumber()},
                {"contactName", dataGenerator.GetNextContactName()},
                {"contactPhoneNumber", dataGenerator.GetNextContactPhoneNumber()}
            };
        }
    }
}