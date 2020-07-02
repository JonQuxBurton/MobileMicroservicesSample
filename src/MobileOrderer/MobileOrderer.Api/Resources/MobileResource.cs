using System;

namespace MobileOrderer.Api.Resources
{
    public class MobileResource
    {
        public int Id { get; set; }
        public Guid GlobalId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
