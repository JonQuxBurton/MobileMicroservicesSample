using System.Collections.Generic;
using System.Linq;

namespace LoadTestingWebService
{
    public class ScenariosFactory : IScenariosFactory
    {
        private readonly IDataGenerator dataGenerator;

        public ScenariosFactory(IDataGenerator dataGenerator)
        {
            this.dataGenerator = dataGenerator;
        }

        public List<Scenario> GetScenarios(ScenariosSettings scenariosSettings)
        {
            var createCustomerSettings = scenariosSettings.Scenarios.FirstOrDefault(x => x.Name == "CreateCustomer");
            var orderMobileSettings = scenariosSettings.Scenarios.FirstOrDefault(x => x.Name == "OrderMobile");
            var completeProvisionSettings =
                scenariosSettings.Scenarios.FirstOrDefault(x => x.Name == "CompleteProvision");
            var completeActivateSettings =
                scenariosSettings.Scenarios.FirstOrDefault(x => x.Name == "CompleteActivate");
            var activateMobileSettings = scenariosSettings.Scenarios.FirstOrDefault(x => x.Name == "ActivateMobile");

            var scenarios = new List<Scenario>
            {
                new CreateCustomerScenario(createCustomerSettings),
                new OrderMobileScenario(orderMobileSettings, dataGenerator),
                new CompleteProvisionScenario(completeProvisionSettings, dataGenerator),
                new CompleteActivateScenario(completeActivateSettings, dataGenerator),
                new ActivateMobileScenario(activateMobileSettings, dataGenerator)
            };

            return scenarios;
        }
    }
}