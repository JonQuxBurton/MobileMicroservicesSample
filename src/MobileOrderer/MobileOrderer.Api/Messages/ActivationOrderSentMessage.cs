using MinimalEventBus.JustSaying;
using System;

namespace MobileOrderer.Api.Messages
{
    public class ActivationOrderSentMessage : Message
    {
        public Guid MobileOrderId { get; set; }
    }
}
