using MinimalEventBus.JustSaying;
using System;

namespace SimCards.EventHandlers.Messages
{
    public class ProvisioningOrderCompletedMessage : Message
    {
        public Guid MobileOrderId { get; set; }
    }
}