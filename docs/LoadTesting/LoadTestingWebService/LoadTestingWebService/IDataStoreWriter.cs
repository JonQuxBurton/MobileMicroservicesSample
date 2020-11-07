using System.Collections.Generic;

namespace LoadTestingWebService
{
    public interface IDataStoreWriter
    {
        Dictionary<string, ScenarioData> Write(List<Scenario> scenarios, Dictionary<string, ScenarioData> data);
    }
}