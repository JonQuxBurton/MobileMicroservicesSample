using MinimalEventBus.JustSaying;
using System;

namespace SimCards.EventHandlers
{
    public class ProvisioningOrderCompletedMessage : Message
    {
        public Guid MobileOrderId { get; set; }
    }
}