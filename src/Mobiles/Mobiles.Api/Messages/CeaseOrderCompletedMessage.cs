using MinimalEventBus.JustSaying;
using System;

namespace Mobiles.Api.Messages
{
    public class CeaseOrderCompletedMessage : Message
    {
        public Guid MobileOrderId { get; set; }
    }
}
