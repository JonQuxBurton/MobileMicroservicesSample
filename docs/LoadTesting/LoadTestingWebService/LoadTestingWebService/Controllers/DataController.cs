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
}