using System.Collections.Generic;

namespace LoadTestingWebService
{
    public interface IScenariosDataBuilder
    {
        List<DataForScenario> Build(List<Scenario> scenarios);
    }
}