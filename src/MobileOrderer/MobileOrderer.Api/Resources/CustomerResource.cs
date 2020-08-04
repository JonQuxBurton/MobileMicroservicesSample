using System;

namespace MobileOrderer.Api.Resources
{
    internal class CustomerResource
    {
        public Guid GlobalId { get; set; }
        public string Nameof { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}