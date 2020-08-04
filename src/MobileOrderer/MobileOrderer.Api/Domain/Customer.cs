using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Utils.DomainDrivenDesign;

namespace MobileOrderer.Api.Domain
{
    [Table("Customers", Schema = "MobileOrderer")]
    public class Customer : AggregateRoot
    {
        public Guid GlobalId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Name { get; set; }

        public List<MobileDataEntity> Mobiles { get; set; }
    }
}
