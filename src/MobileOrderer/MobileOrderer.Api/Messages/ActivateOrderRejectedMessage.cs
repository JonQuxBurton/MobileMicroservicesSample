using MinimalEventBus.JustSaying;
using System;

namespace MobileOrderer.Api.Messages
{
    public class ActivateOrderRejectedMessage : Message
    {
        public string PhoneNumber { get; set; }
        public Guid MobileOrderId { get; set; }
        public string Reason { get; set; }
    }
}
