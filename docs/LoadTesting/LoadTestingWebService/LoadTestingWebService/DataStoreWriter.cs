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

        public List<DataForScenario> Write(List<Scenario> scenarios, List<DataForScenario> data)
        {
            var scenariosWhichRequireDataInDatabase =
                scenarios.Where(x => x.RequiresDataInDatabase).Select(y => y.GetType().Name.Replace("Scenario", ""))
                    .ToList();

            foreach (var scenarioToWrite in scenariosWhichRequireDataInDatabase)
            {
                var dataForScenario = data.FirstOrDefault(x => x.ScenarioName == scenarioToWrite);

                if (dataForScenario != null)
                    WriteScenario(scenarioToWrite, dataForScenario.Data);
            }

            return data;
        }

        private void WriteScenario(string scenarioToWrite, List<DataForIteration> data)
        {
            foreach (var dataForVirtualUsers in data)
            foreach (var dataForIteration in dataForVirtualUsers.Data)
            {
                var newMobileDbId = dataStore.SetupData(scenarioToWrite, dataForIteration);
                dataForIteration.Add("mobileDbId", newMobileDbId.ToString());
            }
        }
    }
}