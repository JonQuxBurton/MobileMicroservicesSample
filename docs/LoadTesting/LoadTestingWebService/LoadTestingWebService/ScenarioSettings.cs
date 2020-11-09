namespace LoadTestingWebService
{
    public class ScenarioSettings
    {
        public string Name { get; set; }
        public int VirtualUsers { get; set; }
        public int Iterations { get; set; }
        public bool RequiresData { get; set; }
        public bool RequiresDataInDatabase { get; set; }
    }
}