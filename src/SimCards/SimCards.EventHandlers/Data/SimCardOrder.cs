using System;

namespace SimCards.EventHandlers.Data
{
    public class SimCardOrder
    {
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public Guid MobileId { get; set; }
        public Guid MobileOrderId { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int Attempts { get; set; }
    }
}