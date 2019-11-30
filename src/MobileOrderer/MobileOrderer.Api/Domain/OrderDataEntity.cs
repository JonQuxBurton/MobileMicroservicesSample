using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MobileOrderer.Api.Domain
{
    [Table("Orders", Schema = "MobileOrderer")]
    public class OrderDataEntity
    {
        public int Id { get; set; }
        public Guid GlobalId { get; set; }
        public string Name { get; set; }
        public string ContactPhoneNumber { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public int MobileId { get; set; }
        public MobileDataEntity Mobile { get; set; }
    }
}
