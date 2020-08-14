using MinimalEventBus.JustSaying;
using System;

namespace Mobiles.Api.Messages
{
    public class ActivateOrderCompletedMessage : Message
    {
        public Guid MobileOrderId { get; set; }
    }
}
