using MinimalEventBus.JustSaying;
using System;

namespace MobileTelecomsNetwork.EventHandlers.Messages
{
    public class ActivateOrderRejectedMessage : Message
    {
        public string PhoneNumber { get; set; }
        public Guid MobileOrderId { get; set; }
        public string Reason { get; set; }
    }
}