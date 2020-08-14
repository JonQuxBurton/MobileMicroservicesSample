using MinimalEventBus.JustSaying;
using System;

namespace Mobiles.Api.Messages
{
    public class ProvisionOrderSentMessage : Message
    {
        public Guid MobileOrderId { get; set; }
    }
}
