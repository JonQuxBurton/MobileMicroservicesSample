using MinimalEventBus.JustSaying;
using System;

namespace SimCards.EventHandlers.Messages
{
    public class ProvisionRequestedMessage : Message
    {
        public string PhoneNumber{ get; set; }
        public Guid MobileId { get; set; }
        public Guid MobileOrderId { get; set; }
        public string Name { get; set; }
        public string ContactPhoneNumber { get; set; }
    }
}
