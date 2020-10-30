using LoadTestingWebService.Controllers;

namespace LoadTestingWebService
{
    public interface IScenarioLogger
    {
        bool Log(ScenarioLog scenarioLog);
        bool BeginLog(ScenarioLog scenarioLog);
    }
}