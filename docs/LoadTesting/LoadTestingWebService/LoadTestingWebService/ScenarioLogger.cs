using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using LoadTestingWebService.Controllers;
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

        public bool BeginLog(ScenarioLog scenarioLog)
        {
            var filePath = GetFilePath(scenarioLog);
            var lines = new List<string>
            {
                DateTime.Now.ToString(CultureInfo.InvariantCulture),
                $"Scenario: {scenarioLog.ScenarioKey}, VirtualUserId: {scenarioLog.VirtualUserId}, Index0: {scenarioLog.Index0}, Iteration: {scenarioLog.Iteration}"
            };

            File.WriteAllLines(filePath, lines);

            return true;
        }

        public bool Log(ScenarioLog scenarioLog)
        {
            var filePath = GetFilePath(scenarioLog);

            var lines = new List<string> {scenarioLog.Message};
            File.AppendAllLines(filePath, lines);

            return true;
        }

        private string GetFilePath(ScenarioLog scenarioLog)
        {
            return $"{testDataSettings.Path}\\ScenarioLog-{scenarioLog.ScenarioKey}_{scenarioLog.Index0}_{scenarioLog.Iteration}.txt";
        }
    }
}
