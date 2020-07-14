using MinimalEventBus.JustSaying;
using System;

namespace MobileTelecomsNetwork.EventHandlers.Messages
{
    public class CeaseOrderSentMessage : Message
    {
        public Guid MobileOrderId { get; set; }
    }
}
