using MinimalEventBus.JustSaying;
using System;

namespace Mobiles.Api.Messages
{
    public class ActivateOrderRejectedMessage : Message
    {
        public string PhoneNumber { get; set; }
        public Guid MobileOrderId { get; set; }
        public string Reason { get; set; }
    }
}
