using System;
using System.Collections.Generic;
using System.Linq;

namespace LoadTestingWebService
{
    public class DataForScenario
    {
        public DataForScenario(string scenarioName, List<DataForIteration> data)
        {
            ScenarioName = scenarioName;
            Data = data;
        }

        public string ScenarioName { get; }
        public List<DataForIteration> Data { get; }

        public List<Guid> GetUserGlobalIds()
        {
            return Data.Select(x => x.UserId).ToList();
        }
    }
}