using System;

namespace EndToEndApiLevelTests
{
    public class Config
    {
        public Config()
        {
            ConnectionString = "Server=localhost,5433;Database=Mobile;User Id=SA;Password=Pass@word";
            FinalActionCheckDelay = TimeSpan.FromSeconds(10);
        }

        public string ConnectionString { get; private set; }
        public TimeSpan FinalActionCheckDelay { get; private set; }
    }
}
