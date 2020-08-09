using System;

namespace ExternalMobileTelecomsNetwork.Api.Resources
{
    public class OrderToAdd
    {
        public Guid MobileReference { get; set; }
        public string PhoneNumber { get; set; }
    }
}
