using MinimalEventBus.JustSaying;
using System;

namespace SimCards.EventHandlers.Messages
{
    public class ProvisionOrderCompletedMessage : Message
    {
        public Guid MobileOrderId { get; set; }
    }
}