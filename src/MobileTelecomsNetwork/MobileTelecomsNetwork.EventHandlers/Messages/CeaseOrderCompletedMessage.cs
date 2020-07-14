using MinimalEventBus.JustSaying;
using System;

namespace MobileTelecomsNetwork.EventHandlers.Messages
{
    public class CeaseOrderCompletedMessage : Message
    {
        public Guid MobileOrderId { get; set; }
    }
}