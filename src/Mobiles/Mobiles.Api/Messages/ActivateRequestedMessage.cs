using MinimalEventBus.JustSaying;
using System;

namespace Mobiles.Api.Messages
{
    public class ActivateRequestedMessage : Message
    {
        public string PhoneNumber { get; set; }
        public Guid MobileId { get; set; }
        public Guid MobileOrderId { get; set; }
        public string ActivationCode { get; set; }
    }
}
