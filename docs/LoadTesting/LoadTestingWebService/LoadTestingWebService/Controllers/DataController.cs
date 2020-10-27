using System;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;

namespace LoadTestingWebService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private static readonly ConcurrentDictionary<string, Scenario>
            Scenarios = new ConcurrentDictionary<string, Scenario>();

        [HttpPost]
        public int Post(ScenarioKeyRequest scenarioKeyRequest)
        {
            var scenarioKey = scenarioKeyRequest.ScenarioKey;

            if (!Scenarios.ContainsKey(scenarioKey))
                Scenarios.TryAdd(scenarioKey, new Scenario(scenarioKey));

            return Scenarios[scenarioKey].GetIndex0(scenarioKeyRequest.VuId);
        }
    }

    public class Scenario
    {
        private readonly string name;

        private static readonly ConcurrentDictionary<int, int>
            VuIdToIndex0 = new ConcurrentDictionary<int, int>();

        private int currentIndex;
        private readonly object currentIndexLock = new object();

        public Scenario(string name)
        {
            this.name = name;
        }

        public int GetIndex0(int vuId)
        {
            if (!VuIdToIndex0.ContainsKey(vuId))
            {
                lock (currentIndexLock)
                {
                    VuIdToIndex0.TryAdd(vuId, currentIndex);
                    Console.WriteLine($"Scenario {name} VuId {vuId} set to index: {currentIndex}");
                    currentIndex++;
                }
            }

            return VuIdToIndex0[vuId];
        }
    }
}