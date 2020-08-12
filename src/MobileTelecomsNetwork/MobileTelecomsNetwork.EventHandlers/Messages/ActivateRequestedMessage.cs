using MinimalEventBus.JustSaying;
using System;

namespace MobileTelecomsNetwork.EventHandlers.Messages
{
    public class ActivateRequestedMessage : Message
    {
        public Guid MobileId { get; set; }
        public Guid MobileOrderId { get; set; }
        public string ActivationCode { get; set; }
        public string PhoneNumber { get; set; }
    }
}
