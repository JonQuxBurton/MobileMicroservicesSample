using System;
using System.Collections.Generic;

namespace LoadTestingWebService
{
    public class ScenariosDataBuilder : IScenariosDataBuilder
    {
        public List<DataForScenario> Build(List<Scenario> scenarios)
        {
            var list = new List<DataForScenario>();
            foreach (var scenario in scenarios)
            {
                var dataForScenario = BuildForScenario(scenario);
                var scenarioName = scenario.GetType().Name.Replace("Scenario", "");
                
                list.Add(new DataForScenario(scenarioName, dataForScenario));
            }
            
            return list;
        }

        private List<DataForIteration> BuildForScenario(Scenario scenario)
        {
            var dataForIterationList = new List<DataForIteration>();

            for (var i = 0; i < scenario.VirtualUsers; i++)
            {
                var dataForIterations = new Dictionary<string, string>[scenario.Iterations];
                for (var j = 0; j < scenario.Iterations; j++)
                    dataForIterations[j] = scenario.GetData();

                dataForIterationList.Add(new DataForIteration(Guid.NewGuid(), dataForIterations));
            }

            return dataForIterationList;
        }
    }
}