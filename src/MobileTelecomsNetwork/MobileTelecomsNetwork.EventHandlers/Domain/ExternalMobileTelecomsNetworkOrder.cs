using System;

namespace MobileTelecomsNetwork.EventHandlers.Domain
{
    public class ExternalMobileTelecomsNetworkOrder
    {
        public Guid MobileReference { get; set; }
        public string PhoneNumber { get; set; }
    }
}