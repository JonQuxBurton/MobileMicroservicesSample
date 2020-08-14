using MinimalEventBus.JustSaying;
using System;

namespace Mobiles.Api.Messages
{
    public class ProvisionOrderCompletedMessage : Message
    {
        public Guid MobileOrderId { get; set; }
    }
}
