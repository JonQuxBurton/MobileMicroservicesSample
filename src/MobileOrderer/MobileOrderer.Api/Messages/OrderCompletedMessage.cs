using MinimalEventBus.JustSaying;
using System;

namespace MobileOrderer.Api.Messages
{
    public class ProvisioningOrderCompletedMessage : Message
    {
        public Guid MobileOrderId { get; set; }
    }
}
