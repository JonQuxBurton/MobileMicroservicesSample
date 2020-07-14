using MinimalEventBus.JustSaying;
using System;

namespace MobileOrderer.Api.Messages
{
    public class CeaseOrderCompletedMessage : Message
    {
        public Guid MobileOrderId { get; set; }
    }
}
