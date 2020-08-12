using System;

namespace MobileTelecomsNetwork.EventHandlers.Domain
{
    public class ExternalMobileTelecomsNetworkOrder
    {
        public Guid Reference { get; set; }
        public string PhoneNumber { get; set; }
        public string ActivationCode { get; set; }
    }
}