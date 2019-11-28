using MobileOrderer.Api.Resources;
using System;

namespace MobileOrderer.Api.Domain
{
    public class MobileOrder : Entity
    {
        public MobileOrder(Guid globalId, string name, string contactPhoneNumber, string status)
        {
            GlobalId = globalId;
            Name = name;
            ContactPhoneNumber = contactPhoneNumber;
            Status = status;
        }

        public MobileOrder(int id, Guid globalId, int mobileId, string name, string contactPhoneNumber, string status, DateTime createdAt, DateTime? updatedAt)
        {
            Id = id;
            GlobalId = globalId;
            MobileId = mobileId;
            Name = name;
            ContactPhoneNumber = contactPhoneNumber;
            Status = status;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }

        public MobileOrder(MobileOrderToAdd mobileOrderToAdd)
        {
            Name = mobileOrderToAdd.Name;
            ContactPhoneNumber = mobileOrderToAdd.ContactPhoneNumber;
        }

        public Guid GlobalId { get; private set; }
        public int MobileId { get; private set; }
        public string Name { get; private set; }
        public string ContactPhoneNumber { get; private set; }
        public string Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        public void Process()
        {
            Status = "Pending";
        }
    }
}
