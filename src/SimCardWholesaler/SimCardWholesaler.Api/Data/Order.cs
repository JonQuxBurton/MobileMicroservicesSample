using System;

namespace SimCardWholesaler.Api.Data
{
    public class Order
    {
        public int Id { get; set; }
        public Guid Reference { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
