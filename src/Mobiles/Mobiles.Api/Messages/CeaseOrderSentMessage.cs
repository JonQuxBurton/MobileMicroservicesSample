using MinimalEventBus.JustSaying;
using System;

namespace Mobiles.Api.Messages
{
    public class CeaseOrderSentMessage : Message
    {
        public Guid MobileOrderId { get; set; }
    }
}
