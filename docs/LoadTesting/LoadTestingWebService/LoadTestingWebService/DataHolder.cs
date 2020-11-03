using System;
using System.Collections.Generic;

namespace LoadTestingWebService
{
    public class DataHolder
    {
        public Dictionary<Guid, Dictionary<string, string>[]> OrderMobile { get; set; }
        public Dictionary<Guid, Dictionary<string, string>[]> CompleteProvision { get; set; }
        public Dictionary<Guid, Dictionary<string, string>[]> ActivateMobile { get; set; }
        public Dictionary<Guid, Dictionary<string, string>[]> CompleteActivate { get; set; }
    }
}