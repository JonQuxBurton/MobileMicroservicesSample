using System;
using System.Collections.Generic;

namespace Mobiles.Api.Resources
{
    public class MobileResource
    {
        public int Id { get; set; }
        public Guid GlobalId { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string PhoneNumber { get; set; }
        public string State { get; set; }
        public IEnumerable<OrderResource> Orders { get; set; }
    }
}
