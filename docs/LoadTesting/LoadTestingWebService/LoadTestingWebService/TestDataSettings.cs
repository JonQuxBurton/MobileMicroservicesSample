namespace LoadTestingWebService
{
    public class TestDataSettings
    {
        public string ConnectionString { get; set; }
        public string CustomerId { get; set; }
        public string Path { get; set; }
        public string FileNameData { get; set; }
        public string FileNameScenarios { get; set; }
        public ScenarioSettings CreateCustomersSettings { get; set; }
        public ScenarioSettings OrderMobilesSettings { get; set; }
        public ScenarioSettings CompleteProvisionsSettings { get; set; }
        public ScenarioSettings ActivateMobilesSettings { get; set; }
        public ScenarioSettings CompleteActivatesSettings { get; set; }
    }
}
