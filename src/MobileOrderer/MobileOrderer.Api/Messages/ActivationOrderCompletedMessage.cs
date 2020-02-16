using MinimalEventBus.JustSaying;
using System;

namespace MobileOrderer.Api.Messages
{
    public class ActivationOrderCompletedMessage : Message
    {
        public Guid MobileOrderId { get; set; }
    }
}
