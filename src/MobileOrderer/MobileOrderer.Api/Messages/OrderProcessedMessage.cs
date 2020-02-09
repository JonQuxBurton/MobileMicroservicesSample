using MinimalEventBus.JustSaying;
using System;

namespace MobileOrderer.Api.Messages
{
    public class OrderProcessedMessage : Message
    {
        public Guid MobileOrderId { get; set; }
    }
}
