using MinimalEventBus.JustSaying;
using System;

namespace SimCards.EventHandlers.Messages
{
    public class ProvisionOrderSentMessage : Message
    {
        public Guid MobileOrderId { get; set; }
    }
}
