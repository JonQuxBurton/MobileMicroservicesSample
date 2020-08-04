using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MobileOrderer.Api.Domain
{
    [Table("Mobiles", Schema = "MobileOrderer")]
    public class MobileDataEntity
    {
        public int Id { get; set; }
        public Guid GlobalId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Customer Customer { get; set; }

        public string State { get; set; }

        public List<OrderDataEntity> Orders { get; set; }

        public void AddOrder(OrderDataEntity orderDataEntity)
        {
            if (this.Orders == null)
                this.Orders = new List<OrderDataEntity>();

            this.Orders.Add(orderDataEntity);
        }
    }
}
