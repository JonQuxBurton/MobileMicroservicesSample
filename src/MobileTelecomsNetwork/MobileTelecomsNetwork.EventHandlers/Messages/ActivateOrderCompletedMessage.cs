using MinimalEventBus.JustSaying;
using System;

namespace MobileTelecomsNetwork.EventHandlers.Messages
{
    public class ActivateOrderCompletedMessage : Message
    {
        public Guid MobileOrderId { get; set; }
    }
}