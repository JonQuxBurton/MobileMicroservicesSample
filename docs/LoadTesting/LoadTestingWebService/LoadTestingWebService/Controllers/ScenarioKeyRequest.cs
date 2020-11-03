namespace LoadTestingWebService.Controllers
{
    public class ScenarioKeyRequest
    {
        public string ScenarioKey { get; set; }
        public int VirtualUserId { get; set; }
        public int Iteration { get; set; }
    }
}