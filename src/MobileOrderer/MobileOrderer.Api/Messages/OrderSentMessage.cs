using MinimalEventBus.JustSaying;
using System;

namespace MobileOrderer.Api.Messages
{
    public class OrderSentMessage : Message
    {
        public Guid MobileOrderId { get; set; }
    }
}
