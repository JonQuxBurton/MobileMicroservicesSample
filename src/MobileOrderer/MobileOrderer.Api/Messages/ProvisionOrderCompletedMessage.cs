using MinimalEventBus.JustSaying;
using System;

namespace MobileOrderer.Api.Messages
{
    public class ProvisionOrderCompletedMessage : Message
    {
        public Guid MobileOrderId { get; set; }
    }
}
