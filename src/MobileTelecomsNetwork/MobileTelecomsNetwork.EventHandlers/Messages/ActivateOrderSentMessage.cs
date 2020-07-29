using MinimalEventBus.JustSaying;
using System;

namespace MobileTelecomsNetwork.EventHandlers.Messages
{
    public class ActivateOrderSentMessage : Message
    {
        public Guid MobileOrderId { get; set; }
    }
}
