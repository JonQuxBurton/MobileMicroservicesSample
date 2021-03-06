﻿using System;

namespace Mobiles.Api.Resources
{
    public class CustomerResource
    {
        public Guid GlobalId { get; set; }
        public string Name { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public MobileResource[] Mobiles { get; set; } = Array.Empty<MobileResource>();
    }
}