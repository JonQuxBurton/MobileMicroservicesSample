using MinimalEventBus.JustSaying;
using System;

namespace MobileTelecomsNetwork.EventHandlers.Messages
{
    public class ActivateRequestedMessage : Message
    {
        public Guid MobileOrderId { get; set; }
        public string ActivatationCode { get; set; }
    }
}
