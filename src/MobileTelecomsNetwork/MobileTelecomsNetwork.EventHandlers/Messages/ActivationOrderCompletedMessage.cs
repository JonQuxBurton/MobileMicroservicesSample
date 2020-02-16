﻿using MinimalEventBus.JustSaying;
using System;

namespace MobileTelecomsNetwork.EventHandlers.Messages
{
    public class ActivationOrderCompletedMessage : Message
    {
        public Guid MobileOrderId { get; set; }
    }
}