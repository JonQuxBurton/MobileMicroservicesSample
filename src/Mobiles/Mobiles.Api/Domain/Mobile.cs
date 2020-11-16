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
        private readonly IDateTimeCreator dateTimeCreator;

        private Order newOrder;

        public Mobile(IDateTimeCreator dateTimeCreator, MobileDataEntity mobileDataEntity)
        {
            this.mobileDataEntity = mobileDataEntity;
            this.dateTimeCreator = dateTimeCreator;
            behaviour = new MobileBehaviour(mobileDataEntity);
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

        private void ProcessNextAction(string nextAction)
        {
            if (nextAction is null)
                return;

            if (nextAction == "CreateNewOrder" && newOrder != null)
                mobileDataEntity.AddOrder(newOrder.GetDataEntity());
            else if (nextAction == "CompleteInProgressOrder" && InProgressOrder != null)
                InProgressOrder.Complete();
            else if (nextAction == "RejectInProgressOrder" && InProgressOrder != null) InProgressOrder.Reject();
        }

        public void Provision()
        {
            newOrder = InProgressOrder;
            var result = behaviour.Provision();
            mobileDataEntity.State = result.MobileState.ToString();
            ProcessNextAction(result.Action);
            mobileDataEntity.UpdatedAt = dateTimeCreator.Create();
        }

        public void Activate(Order order)
        {
            newOrder = order;
            var result = behaviour.Activate();
            mobileDataEntity.State = result.MobileState.ToString();
            ProcessNextAction(result.Action);
            mobileDataEntity.UpdatedAt = dateTimeCreator.Create();
        }

        public void Cease(Order order)
        {
            newOrder = order;
            var result = behaviour.Cease();
            mobileDataEntity.State = result.MobileState.ToString();
            ProcessNextAction(result.Action);
            mobileDataEntity.UpdatedAt = dateTimeCreator.Create();
        }

        public void ActivateCompleted()
        {
            var result = behaviour.ActivateCompleted();
            mobileDataEntity.State = result.MobileState.ToString();
            ProcessNextAction(result.Action);
            mobileDataEntity.UpdatedAt = dateTimeCreator.Create();
        }

        public void ActivateRejected()
        {
            var result = behaviour.ActivateRejected();
            mobileDataEntity.State = result.MobileState.ToString();
            ProcessNextAction(result.Action);
            mobileDataEntity.UpdatedAt = dateTimeCreator.Create();
        }

        public void ProcessingProvisionCompleted()
        {
            var result = behaviour.ProcessingProvisionCompleted();
            mobileDataEntity.State = result.MobileState.ToString();
            ProcessNextAction(result.Action);
            mobileDataEntity.UpdatedAt = dateTimeCreator.Create();
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
            var result = behaviour.CeaseCompleted();
            mobileDataEntity.State = result.MobileState.ToString();
            ProcessNextAction(result.Action);

            mobileDataEntity.UpdatedAt = DateTime.Now;
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