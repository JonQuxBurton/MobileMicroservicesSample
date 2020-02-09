using MinimalEventBus.JustSaying;
using System;

namespace SimCards.EventHandlers.Messages
{
    public class OrderSentMessage : Message
    {
        public Guid MobileOrderId { get; set; }
    }
}
