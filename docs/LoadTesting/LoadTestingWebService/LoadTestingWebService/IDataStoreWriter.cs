using System.Collections.Generic;

namespace LoadTestingWebService
{
    public interface IDataStoreWriter
    {
        List<DataForScenario> Write(List<Scenario> scenarios, List<DataForScenario> data);
    }
}