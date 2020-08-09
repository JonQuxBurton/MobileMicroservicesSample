using System;

namespace SimCards.EventHandlers.Domain
{
    public class ExternalSimCardOrder
    {
        public string Name { get; set; }
        public Guid MobileReference { get; set; }
        public string PhoneNumber { get; set; }
    }
}