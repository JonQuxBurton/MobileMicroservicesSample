using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mobiles.Api.Domain
{
    [Table("Mobiles", Schema = "Mobiles")]
    public class MobileDataEntity
    {
        public int Id { get; set; }
        public Guid GlobalId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid CustomerId { get; set; }

        public string State { get; set; }
        public string PhoneNumber{ get; set; }

        public List<OrderDataEntity> Orders { get; set; }

        public void AddOrder(OrderDataEntity orderDataEntity)
        {
            if (this.Orders == null)
                this.Orders = new List<OrderDataEntity>();

            this.Orders.Add(orderDataEntity);
        }
    }
}
