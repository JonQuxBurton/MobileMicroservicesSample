using System;

namespace MobileTelecomsNetwork.EventHandlers.Domain
{
    public class Order
    {
        public string Name { get; set; }
        public Guid MobileOrderId { get; set; }
        public OrderStatus Status { get; set; }
        public OrderType Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string ActivationCode { get; set; }
        public string PhoneNumber { get; set; }
        public Guid MobileId { get; set; }
    }
}