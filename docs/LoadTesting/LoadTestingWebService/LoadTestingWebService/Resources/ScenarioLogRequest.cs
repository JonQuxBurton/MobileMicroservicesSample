using System;

namespace LoadTestingWebService.Resources
{
    public class ScenarioLogRequest
    {
        public string ScenarioKey { get; set; }
        public string VirtualUserId { get; set; }
        public string Iteration { get; set; }
        public string Message { get; set; }
        public Guid UserGlobalId { get; set; }
    }
}