using MinimalEventBus.JustSaying;
using System;

namespace MobileTelecomsNetwork.EventHandlers.Messages
{
    public class CeaseRequestedMessage : Message
    {
        public Guid MobileOrderId { get; set; }
        public string PhoneNumber { get; set; }
        public Guid MobileId { get; set; }
    }
}
