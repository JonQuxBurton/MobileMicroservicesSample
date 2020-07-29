using MinimalEventBus.JustSaying;
using System;

namespace MobileOrderer.Api.Messages
{
    public class ActivateOrderCompletedMessage : Message
    {
        public Guid MobileOrderId { get; set; }
    }
}
