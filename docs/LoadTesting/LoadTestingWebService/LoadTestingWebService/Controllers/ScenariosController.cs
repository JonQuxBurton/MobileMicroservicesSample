﻿using LoadTestingWebService.Resources;
using Microsoft.AspNetCore.Mvc;

namespace LoadTestingWebService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ScenariosController : ControllerBase
    {
        private readonly IScenarioLogger scenarioLogger;

        public ScenariosController(IScenarioLogger scenarioLogger)
        {
            this.scenarioLogger = scenarioLogger;
        }

        [HttpPost("beginlog")]
        public bool BeginLog(ScenarioLogRequest scenarioLog)
        {
            return scenarioLogger.BeginLog(scenarioLog);
        }

        [HttpPost("log")]
        public bool Log(ScenarioLogRequest scenarioLog)
        {
            return scenarioLogger.Log(scenarioLog);
        }
    }
}
