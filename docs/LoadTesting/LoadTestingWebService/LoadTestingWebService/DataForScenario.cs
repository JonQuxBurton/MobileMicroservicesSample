using System;
using System.Collections.Generic;
using System.Linq;

namespace LoadTestingWebService
{
    public class DataForScenario
    {
        public string ScenarioName { get; set; }
        public List<DataForIteration> Data { get; set; }

        public List<Guid> GetUserGlobalIds()
        {
            return Data.Select(x => x.UserId).ToList();
        }
    }
}