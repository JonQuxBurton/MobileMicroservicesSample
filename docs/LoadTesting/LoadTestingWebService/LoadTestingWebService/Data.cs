using System;
using System.Collections.Generic;

namespace LoadTestingWebService
{
    public class Data
    {
        public Dictionary<Guid, OrderMobileTestData[]> OrderMobile { get; set; }
        public Dictionary<Guid, CompleteProvisionTestData[]> CompleteProvision { get; set; }
        public Dictionary<Guid, ActivateMobileTestData[]> ActivateMobile { get; set; }
        public Dictionary<Guid, CompleteActivateTestData[]> CompleteActivate { get; set; }
    }
}