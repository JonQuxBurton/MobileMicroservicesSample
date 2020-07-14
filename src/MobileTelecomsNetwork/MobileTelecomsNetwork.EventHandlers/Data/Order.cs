using System;

namespace MobileTelecomsNetwork.EventHandlers.Data
{
    public class Order
    {
        public string Name { get; set; }
        public Guid MobileOrderId { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}