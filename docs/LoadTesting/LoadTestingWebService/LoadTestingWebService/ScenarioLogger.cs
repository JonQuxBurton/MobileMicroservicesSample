using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using LoadTestingWebService.Resources;
using Microsoft.Extensions.Options;

namespace LoadTestingWebService
{
    public class ScenarioLogger : IScenarioLogger
    {
        private readonly TestDataSettings testDataSettings;

        public ScenarioLogger(IOptions<TestDataSettings> sestDataSettingsOptions)
        {
            this.testDataSettings = sestDataSettingsOptions.Value;
        }

        public bool BeginLog(ScenarioLogRequest scenarioLog)
        {
            var filePath = GetFilePath(scenarioLog);
            var lines = new List<string>
            {
                DateTime.Now.ToString(CultureInfo.InvariantCulture),
                $"Scenario: {scenarioLog.ScenarioKey}, VirtualUserId: {scenarioLog.VirtualUserId}, UserGlobalId: {scenarioLog.UserGlobalId}, Iteration: {scenarioLog.Iteration}"
            };

            File.WriteAllLines(filePath, lines);

            return true;
        }

        public bool Log(ScenarioLogRequest scenarioLog)
        {
            var filePath = GetFilePath(scenarioLog);

            var lines = new List<string> {scenarioLog.Message};
            File.AppendAllLines(filePath, lines);

            return true;
        }

        private string GetFilePath(ScenarioLogRequest scenarioLog)
        {
            return $"{testDataSettings.Path}\\ScenarioLog-{scenarioLog.ScenarioKey}_{scenarioLog.UserGlobalId}_{scenarioLog.Iteration}.txt";
        }
    }
}
