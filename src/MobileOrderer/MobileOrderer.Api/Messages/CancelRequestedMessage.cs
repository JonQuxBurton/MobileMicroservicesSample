﻿using MinimalEventBus.JustSaying;
using System;

namespace MobileOrderer.Api.Messages
{
    public class CancelRequestedMessage : Message
    {
        public Guid MobileOrderId { get; set; }
    }
}
