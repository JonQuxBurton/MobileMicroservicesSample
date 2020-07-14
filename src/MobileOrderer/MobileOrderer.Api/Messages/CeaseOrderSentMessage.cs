using MinimalEventBus.JustSaying;
using System;

namespace MobileOrderer.Api.Messages
{
    public class CeaseOrderSentMessage : Message
    {
        public Guid MobileOrderId { get; set; }
    }
}
