using System;
using System.Collections.Generic;
using System.Linq;
using LoadTestingWebService.Data;

namespace LoadTestingWebService
{
    public class DataStoreWriter : IDataStoreWriter
    {
        private readonly IDataStore dataStore;

        public DataStoreWriter(IDataStore dataStore)
        {
            this.dataStore = dataStore;
        }

        public Dictionary<string, ScenarioData> Write(List<Scenario> scenarios, Dictionary<string, ScenarioData> data)
        {
            var scenariosWhichRequireDataInDatabase =
                scenarios.Where(x => x.RequiresDataInDatabase).Select(y => y.GetType().Name.Replace("Scenario", ""))
                    .ToList();

            foreach (var scenarioToWrite in scenariosWhichRequireDataInDatabase)
            {
                WriteScenario(scenarioToWrite, data[scenarioToWrite].Data);
            }

            return data;
        }

        private void WriteScenario(string scenarioToWrite,
            Dictionary<Guid, Dictionary<string, string>[]> data)
        {
            foreach (var dataForVirtualUsers in data.Values)
            foreach (var dataForIteration in dataForVirtualUsers)
            {
                var newMobileDbId =
                    dataStore.SetupData(scenarioToWrite, dataForIteration);
                dataForIteration.Add("mobileDbId", newMobileDbId.ToString());
            }
        }
    }
}