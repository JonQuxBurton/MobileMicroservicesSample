using System;

namespace ExternalMobileTelecomsNetwork.Api.Resources
{
    public class OrderToAdd
    {
        public Guid Reference { get; set; }
        public string PhoneNumber { get; set; }
    }
}
