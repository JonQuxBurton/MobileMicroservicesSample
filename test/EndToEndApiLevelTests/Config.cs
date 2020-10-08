using System;

namespace EndToEndApiLevelTests
{
    public class Config
    {
        public Config()
        {
            // Running against Docker stack
            //ConnectionString = "Server=localhost,5433;Database=Mobile;User Id=SA;Password=Pass@word";
            
            // Running against Localdev
            ConnectionString = "Server=JQB1-2020;Initial Catalog=Mobile;Integrated Security=True";
            FinalActionCheckDelay = TimeSpan.FromSeconds(10);
        }

        public string ConnectionString { get; private set; }
        public TimeSpan FinalActionCheckDelay { get; private set; }
    }
}
