using System;

namespace ExternalSimCardsProvider.Api.Resources
{
    public class OrderToAdd
    {
        public Guid Reference { get; set; }
        public string Name { get; set; }
    }
}
