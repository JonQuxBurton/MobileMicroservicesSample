using MinimalEventBus.JustSaying;
using System;

namespace SimCards.EventHandlers.Messages
{
    public class MobileRequestedMessage : Message
    {
        public Guid MobileOrderId { get; set; }
        public string Name { get; set; }
        public string ContactPhoneNumber { get; set; }
    }
}
