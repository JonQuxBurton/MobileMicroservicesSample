using System.Collections.Generic;

namespace LoadTestingWebService
{
    public interface IScenariosDataBuilder
    {
        Dictionary<string, ScenarioData> Build(List<Scenario> scenarios);
    }
}