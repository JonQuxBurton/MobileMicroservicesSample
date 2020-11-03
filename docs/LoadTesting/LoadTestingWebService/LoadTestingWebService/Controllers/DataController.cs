using System;
using LoadTestingWebService.Resources;
using Microsoft.AspNetCore.Mvc;

namespace LoadTestingWebService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly IScenariosService scenariosService;

        public DataController(IScenariosService scenariosService)
        {
            this.scenariosService = scenariosService;
        }

        [HttpPost]
        public UserResource Post(ScenarioKeyRequest scenarioKeyRequest)
        {
            var scenarioKey = scenarioKeyRequest.ScenarioKey;
            var user = scenariosService.GetUserId(scenarioKey, scenarioKeyRequest.VirtualUserId);

            Console.WriteLine($"User returned - ScenarioKey:{scenarioKeyRequest.ScenarioKey}, VirtualUserId: {scenarioKeyRequest.VirtualUserId} { user.GlobalId}");

            return user;
        }
    }
}