using System.Collections.Generic;

namespace LoadTestingWebService
{
    public abstract class Scenario
    {
        protected Scenario(int virtualUsers, int iterations, bool requiresData, bool requiresDataInDatabase)
        {
            RequiresDataInDatabase = requiresDataInDatabase;
            VirtualUsers = virtualUsers;
            Iterations = iterations;
            RequiresData = requiresData;
        }

        public int VirtualUsers { get; }
        public int Iterations { get; }
        public bool RequiresData { get; }
        public bool RequiresDataInDatabase { get; }

        public abstract Dictionary<string, string> GetData();
    }
}