using System;

namespace ExternalSimCardsProvider.Api.Resources
{
    public class OrderToAdd
    {
        public string PhoneNumber { get; set; }
        public Guid Reference { get; set; }
        public string Name { get; set; }
    }
}
