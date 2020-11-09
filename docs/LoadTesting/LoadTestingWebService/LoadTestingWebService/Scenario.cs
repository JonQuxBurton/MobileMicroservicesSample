using System.Collections.Generic;

namespace LoadTestingWebService
{
    public abstract class Scenario
    {
        protected Scenario(ScenarioSettings scenarioSettings)
        {
            VirtualUsers = scenarioSettings.VirtualUsers;
            Iterations = scenarioSettings.Iterations;
            RequiresData = scenarioSettings.RequiresData;
            RequiresDataInDatabase = scenarioSettings.RequiresDataInDatabase;
        }

        public int VirtualUsers { get; }
        public int Iterations { get; }
        public bool RequiresData { get; }
        public bool RequiresDataInDatabase { get; }

        public abstract Dictionary<string, string> GetData();
    }
}