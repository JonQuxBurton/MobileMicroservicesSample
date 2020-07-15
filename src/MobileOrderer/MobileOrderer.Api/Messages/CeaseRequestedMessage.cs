using MinimalEventBus.JustSaying;
using System;

namespace MobileOrderer.Api.Messages
{
    public class CeaseRequestedMessage : Message
    {
        public Guid MobileOrderId { get; set; }
    }
}
