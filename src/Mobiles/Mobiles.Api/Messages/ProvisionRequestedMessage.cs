using MinimalEventBus.JustSaying;
using System;

namespace Mobiles.Api.Messages
{
    public class ProvisionRequestedMessage : Message
    {
        public string PhoneNumber { get; set; }
        public Guid MobileId { get; set; }
        public Guid MobileOrderId { get; set; }
        public string Name { get; set; }
        public string ContactPhoneNumber { get; set; }
    }
}
