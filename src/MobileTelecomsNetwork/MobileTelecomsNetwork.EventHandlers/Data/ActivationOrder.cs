using System;

namespace MobileTelecomsNetwork.EventHandlers.Data
{
    public class ActivationOrder
    {
        public string Name { get; set; }
        public Guid MobileOrderId { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}