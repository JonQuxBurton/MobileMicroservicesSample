using MinimalEventBus.JustSaying;
using System;

namespace MobileOrderer.Api.Messages
{
    public class CeaselRequestedMessage : Message
    {
        public Guid MobileOrderId { get; set; }
    }
}
