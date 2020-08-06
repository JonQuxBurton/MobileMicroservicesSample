using MinimalEventBus.JustSaying;
using System;

namespace MobileOrderer.Api.Messages
{
    public class ActivateRequestedMessage : Message
    {
        public Guid MobileOrderId { get; set; }
        public string ActivationCode { get; set; }
    }
}
