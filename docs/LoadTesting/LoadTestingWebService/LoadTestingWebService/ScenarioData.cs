using System;
using System.Collections.Generic;

namespace LoadTestingWebService
{
    public class ScenarioData
    {
        public ScenarioData(Dictionary<Guid, Dictionary<string, string>[]> data,
            List<Guid> userGlobalIds)
        {
            Data = data;
            UserGlobalIds = userGlobalIds;
        }

        public Dictionary<Guid, Dictionary<string, string>[]> Data { get; }
        public List<Guid> UserGlobalIds { get; }
    }
}