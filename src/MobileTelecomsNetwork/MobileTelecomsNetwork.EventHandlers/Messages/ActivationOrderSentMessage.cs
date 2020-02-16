using MinimalEventBus.JustSaying;
using System;

namespace MobileTelecomsNetwork.EventHandlers.Messages
{
    public class ActivationOrderSentMessage : Message
    {
        public Guid MobileOrderId { get; set; }
    }
}
