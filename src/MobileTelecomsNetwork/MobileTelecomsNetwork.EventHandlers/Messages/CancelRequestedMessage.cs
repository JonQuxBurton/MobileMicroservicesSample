﻿using MinimalEventBus.JustSaying;
using System;

namespace MobileTelecomsNetwork.EventHandlers.Messages
{
    public class CancelRequestedMessage : Message
    {
        public Guid MobileOrderId { get; set; }
    }
}
