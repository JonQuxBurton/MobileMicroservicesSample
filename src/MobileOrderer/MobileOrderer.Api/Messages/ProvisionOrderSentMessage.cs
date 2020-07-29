using MinimalEventBus.JustSaying;
using System;

namespace MobileOrderer.Api.Messages
{
    public class ProvisionOrderSentMessage : Message
    {
        public Guid MobileOrderId { get; set; }
    }
}
