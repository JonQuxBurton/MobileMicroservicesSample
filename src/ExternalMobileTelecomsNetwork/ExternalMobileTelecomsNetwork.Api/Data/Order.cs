﻿using System;

namespace ExternalMobileTelecomsNetwork.Api.Data
{
    public class Order
    {
        public int Id { get; set; }
        public Guid Reference { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string PhoneNumber { get; set; }
        public string ActivationCode { get; set; }
        public string Reason { get; set; }
    }
}
