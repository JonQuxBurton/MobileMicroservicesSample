using MinimalEventBus.JustSaying;
using System;

namespace Mobiles.Api.Messages
{
    public class ActivateOrderSentMessage : Message
    {
        public Guid MobileOrderId { get; set; }
    }
}
