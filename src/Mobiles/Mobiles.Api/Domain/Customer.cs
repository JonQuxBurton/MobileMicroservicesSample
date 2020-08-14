using System;
using System.ComponentModel.DataAnnotations.Schema;
using Utils.DomainDrivenDesign;

namespace Mobiles.Api.Domain
{
    [Table("Customers", Schema = "Mobiles")]
    public class Customer : AggregateRoot
    {
        public Guid GlobalId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Name { get; set; }
    }
}
