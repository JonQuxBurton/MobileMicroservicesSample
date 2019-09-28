using System;

namespace SimCardWholesaler.Api.Resources
{
    public class OrderToAdd
    {
        public Guid Reference { get; set; }
        public string Name { get; set; }
    }
}
