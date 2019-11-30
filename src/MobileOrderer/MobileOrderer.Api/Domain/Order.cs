using System;
using Utils.DomainDrivenDesign;

namespace MobileOrderer.Api.Domain
{
    public class Order : Entity
    {
        private readonly OrderDataEntity orderDataEntity;

        public Order(OrderDataEntity orderDataEntity)
        {
            this.orderDataEntity = orderDataEntity;
        }

        public Guid GlobalId => this.orderDataEntity.GlobalId;
        public int MobileId => this.orderDataEntity.MobileId;
        public string Name => this.orderDataEntity.Name;
        public string ContactPhoneNumber => this.orderDataEntity.ContactPhoneNumber;
        public string Status => this.orderDataEntity.Status;
        public DateTime? CreatedAt => this.orderDataEntity.CreatedAt;
        public DateTime? UpdatedAt => this.orderDataEntity.UpdatedAt;

        public void Process()
        {
            this.orderDataEntity.Status = "Pending";
        }
    }
}
