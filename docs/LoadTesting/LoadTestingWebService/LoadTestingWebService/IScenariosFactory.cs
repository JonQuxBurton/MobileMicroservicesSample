using System.Collections.Generic;

namespace LoadTestingWebService
{
    public interface IScenariosFactory
    {
        List<Scenario> GetScenarios(ScenariosSettings scenariosSettings);
    }
}