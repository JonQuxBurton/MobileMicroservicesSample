using System;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;

namespace LoadTestingWebService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly IScenariosService scenariosService;

        private static readonly ConcurrentDictionary<string, Scenario>
            Scenarios = new ConcurrentDictionary<string, Scenario>();

        public DataController(IScenariosService scenariosService)
        {
            this.scenariosService = scenariosService;
        }

        [HttpPost]
        public User Post(ScenarioKeyRequest scenarioKeyRequest)
        {
            var scenarioKey = scenarioKeyRequest.ScenarioKey;
            var user = scenariosService.GetUserId(scenarioKey, scenarioKeyRequest.VirtualUserId);

            Console.WriteLine($"User returned - ScenarioKey:{scenarioKeyRequest.ScenarioKey}, VirtualUserId: {scenarioKeyRequest.VirtualUserId} { user.GlobalId}");

            return user;
        }
    }
}