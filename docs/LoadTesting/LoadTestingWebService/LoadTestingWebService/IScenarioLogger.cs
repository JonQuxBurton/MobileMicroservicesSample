using LoadTestingWebService.Controllers;
using LoadTestingWebService.Resources;

namespace LoadTestingWebService
{
    public interface IScenarioLogger
    {
        bool Log(ScenarioLogRequest scenarioLog);
        bool BeginLog(ScenarioLogRequest scenarioLog);
    }
}