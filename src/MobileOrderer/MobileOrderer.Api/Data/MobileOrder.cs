using MobileOrderer.Api.Resources;
using System;

namespace MobileOrderer.Api.Data
{
    public class MobileOrder
    {
        public MobileOrder()
        {

        }

        public MobileOrder(MobileOrderToAdd mobileOrderToAdd)
        {
            Name = mobileOrderToAdd.Name;
            ContactPhoneNumber = mobileOrderToAdd.ContactPhoneNumber;
        }

        public string Name { get; set; }
        public string ContactPhoneNumber { get; set; }
        public string Status { get; set; }
        public Guid GlobalId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
