using MinimalEventBus.JustSaying;
using System;

namespace MobileTelecomsNetwork.EventHandlers.Messages
{
    public class CancelOrderSentMessage : Message
    {
        public Guid MobileOrderId { get; set; }
    }
}
