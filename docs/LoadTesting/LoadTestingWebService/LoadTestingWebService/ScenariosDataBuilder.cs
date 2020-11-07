using System;
using System.Collections.Generic;
using System.Linq;

namespace LoadTestingWebService
{
    public class ScenariosDataBuilder : IScenariosDataBuilder
    {
        public Dictionary<string, ScenarioData> Build(List<Scenario> scenarios)
        {
            var dataForScenarios = new Dictionary<string, ScenarioData>();
            foreach (var scenario in scenarios)
            {
                var dataForScenario = BuildForScenario(scenario);
                var scenarioName = scenario.GetType().Name.Replace("Scenario", "");
                dataForScenarios.Add(scenarioName, dataForScenario);
            }

            return dataForScenarios;
        }

        private ScenarioData BuildForScenario(Scenario scenario)
        {
            var data = new Dictionary<Guid, Dictionary<string, string>[]>();

            for (var i = 0; i < scenario.VirtualUsers; i++)
            {
                var dataForIterations = new Dictionary<string, string>[scenario.Iterations];
                data.Add(Guid.NewGuid(), dataForIterations);

                for (var j = 0; j < scenario.Iterations; j++)
                    dataForIterations[j] = scenario.GetData();
            }

            return new ScenarioData(data, data.Keys.ToList());
        }
    }
}