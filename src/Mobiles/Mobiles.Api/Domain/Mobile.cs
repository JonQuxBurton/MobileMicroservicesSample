using System;
using System.Collections.Generic;
using System.Linq;
using Utils.DateTimes;
using Utils.DomainDrivenDesign;
using Utils.Enums;

namespace Mobiles.Api.Domain
{
    public class Mobile : AggregateRoot
    {
        public enum MobileState
        {
            New,
            ProcessingProvision,
            WaitingForActivate,
            ProcessingActivate,
            Live,
            ProcessingPortIn,
            Suspended,
            ProcessingCease,
            ProcessingPortOut,
            Ceased,
            PortedOut
        }

        public enum Trigger
        {
            Provision,
            ProcessingProvisionCompleted,
            PortIn,
            Activate,
            ActivateCompleted,
            ActivateRejected,
            PortInCompleted,
            Cease,
            Suspend,
            ReplaceSim,
            Resume,
            RequestPac,
            CeaseCompleted,
            PortOutCompleted
        }

        private readonly MobileBehaviour behaviour;

        private readonly MobileDataEntity mobileDataEntity;

        public Mobile(IDateTimeCreator dateTimeCreator, MobileDataEntity mobileDataEntity)
        {
            this.mobileDataEntity = mobileDataEntity;
            behaviour = new MobileBehaviour(dateTimeCreator, mobileDataEntity);
        }

        public override int Id
        {
            get => mobileDataEntity.Id;
            protected set => base.Id = value;
        }

        public Guid GlobalId => mobileDataEntity.GlobalId;
        public DateTime? CreatedAt => mobileDataEntity.CreatedAt;
        public DateTime? UpdatedAt => mobileDataEntity.UpdatedAt;
        public Guid CustomerId => mobileDataEntity.CustomerId;

        public MobileState State
        {
            get
            {
                var enumConverter = new EnumConverter();
                return enumConverter.ToEnum<MobileState>(mobileDataEntity.State);
            }
        }

        public PhoneNumber PhoneNumber => new PhoneNumber(mobileDataEntity.PhoneNumber);

        public IEnumerable<Order> Orders
        {
            get
            {
                if (mobileDataEntity.Orders is null)
                    return new List<Order>();

                return mobileDataEntity.Orders
                    .OrderByDescending(x => x.CreatedAt)
                    .Select(x => new Order(x));
            }
        }

        public Order InProgressOrder
        {
            get
            {
                return Orders.OrderByDescending(x => x.CreatedAt)
                    .FirstOrDefault(x => x.CurrentState == Order.State.New ||
                                         x.CurrentState == Order.State.Processing ||
                                         x.CurrentState == Order.State.Sent);
            }
        }

        public MobileDataEntity GetDataEntity()
        {
            return mobileDataEntity;
        }

        public void Provision()
        {
            behaviour.Provision(mobileDataEntity, InProgressOrder);
        }


        public void ProcessingProvisionCompleted()
        {
            behaviour.ProcessingProvisionCompleted(mobileDataEntity, InProgressOrder);
        }

        public void Activate(Order order)
        {
            behaviour.Activate(mobileDataEntity, order);
        }

        public void ActivateCompleted()
        {
            behaviour.ActivateCompleted(mobileDataEntity, InProgressOrder);
        }

        public void ActivateRejected()
        {
            behaviour.ActivateRejected(mobileDataEntity, InProgressOrder);
        }

        public void Cease(Order order)
        {
            behaviour.Cease(mobileDataEntity, order);
        }

        public void PortIn()
        {
            behaviour.PortIn();
        }

        public void PortInCompleted()
        {
            behaviour.PortInCompleted();
        }

        public void Suspend()
        {
            behaviour.Suspend();
        }

        public void ReplaceSim()
        {
            behaviour.ReplaceSim();
        }

        public void Resume()
        {
            behaviour.Resume();
        }

        public void RequestPac()
        {
            behaviour.RequestPac();
        }

        public void CeaseCompleted()
        {
            behaviour.CeaseCompleted(mobileDataEntity, InProgressOrder);
        }

        public void PortOutCompleted()
        {
            behaviour.PortOutCompleted();
        }

        public void OrderProcessing()
        {
            InProgressOrder?.Process();
        }

        public void OrderSent()
        {
            InProgressOrder?.Send();
        }
    }
}