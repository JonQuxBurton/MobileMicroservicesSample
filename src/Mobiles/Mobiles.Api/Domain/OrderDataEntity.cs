using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mobiles.Api.Domain
{
    [Table("Orders", Schema = "Mobiles")]
    public class OrderDataEntity
    {
        public int Id { get; set; }
        public Guid GlobalId { get; set; }
        public string Name { get; set; }
        public string ContactPhoneNumber { get; set; }
        public string State { get; set; }
        public string Type { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public int MobileId { get; set; }
        public MobileDataEntity Mobile { get; set; }
        public string ActivationCode { get; set; }
    }
}
