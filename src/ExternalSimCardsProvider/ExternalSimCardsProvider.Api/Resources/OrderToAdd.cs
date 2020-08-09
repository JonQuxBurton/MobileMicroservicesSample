using System;

namespace ExternalSimCardsProvider.Api.Resources
{
    public class OrderToAdd
    {
        public string PhoneNumber { get; set; }
        public Guid MobileReference { get; set; }
        public string Name { get; set; }
    }
}
