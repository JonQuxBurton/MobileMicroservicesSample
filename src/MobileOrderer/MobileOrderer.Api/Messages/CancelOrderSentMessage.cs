using MinimalEventBus.JustSaying;
using System;

namespace MobileOrderer.Api.Messages
{
    public class CancelOrderSentMessage : Message
    {
        public Guid MobileOrderId { get; set; }
    }
}
