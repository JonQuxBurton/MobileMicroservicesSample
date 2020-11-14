using System;
using System.Collections.Generic;
using System.Linq;
using Utils.DomainDrivenDesign;
using Utils.Enums;

namespace Mobiles.Api.Domain
{
    public class Mobile : AggregateRoot
    {
        public enum MobileState { New, ProcessingProvision, WaitingForActivate, ProcessingActivate, Live, ProcessingPortIn, Suspended, ProcessingCease, ProcessingPortOut, Ceased, PortedOut }
        public enum Trigger { Provision, ProcessingProvisionCompleted, PortIn, Activate, ActivateCompleted, ActivateRejected, PortInCompleted, Cease, Suspend, ReplaceSim, Resume, RequestPac, CeaseCompleted, PortOutCompleted }

        public override int Id { get => mobileDataEntity.Id; protected set => base.Id = value; }
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
        public IEnumerable<Order> Orders => orders;
        public Order InFlightOrder { get; private set; }

        //private readonly StateMachine<MobileState, Trigger> machine;
        private readonly MobileDataEntity mobileDataEntity;
        private readonly List<Order> orders;
        //private Order newOrder;
        // bool isOrderRejected = false;
        private readonly MobileBehaviour behaviour;

        public Mobile(MobileDataEntity mobileDataEntity, Order inFlightOrder, IEnumerable<Order> orderHistory = null)
        {
            this.mobileDataEntity = mobileDataEntity;
            InFlightOrder = inFlightOrder;

            if (orderHistory == null)
                orders = new List<Order>();
            else
                orders = orderHistory.OrderByDescending(x => x.CreatedAt).ToList();

            behaviour = new MobileBehaviour(mobileDataEntity, InFlightOrder);
        }

        public MobileDataEntity GetDataEntity()
        {
            return mobileDataEntity;
        }

        public void Provision(Order order)
        {
            var result = behaviour.Provision(order);
            //newOrder = result.InFlightOrder;
            InFlightOrder = result.InFlightOrder;
            mobileDataEntity.State = result.NewMobileState.ToString();
            //this.mobileDataEntity.AddOrder(result.InFlightOrder.GetDataEntity());
            mobileDataEntity.UpdatedAt = DateTime.Now;

            //this.newOrder = order;
            //this.machine.Fire(Trigger.Provision); 
        }
        public void Activate(Order order)
        {
            var result = behaviour.Activate(order, mobileDataEntity);
            //newOrder = result.InFlightOrder;
            InFlightOrder = result.InFlightOrder;
            mobileDataEntity.State = result.NewMobileState.ToString();
            //CreateNewOrder();
            //this.mobileDataEntity.AddOrder(result.InFlightOrder.GetDataEntity());
            mobileDataEntity.UpdatedAt = DateTime.Now;

            //this.newOrder = order;
            //this.machine.Fire(Trigger.Activate);
        }
        public void Cease(Order order)
        {
            var result = behaviour.Cease(order, mobileDataEntity);
            //newOrder = result.InFlightOrder;
            InFlightOrder = result.InFlightOrder;
            mobileDataEntity.State = result.NewMobileState.ToString();
            //this.mobileDataEntity.AddOrder(result.InFlightOrder.GetDataEntity());
            mobileDataEntity.UpdatedAt = DateTime.Now;

            //this.newOrder = order;
            //this.machine.Fire(Trigger.Cease);
        }

        public void ActivateCompleted()
        {
            var result = behaviour.ActivateCompleted(orders);
            //newOrder = result.InFlightOrder;
            InFlightOrder = result.InFlightOrder;
            mobileDataEntity.State = result.NewMobileState.ToString();
            //CompleteInFlightOrder();
            mobileDataEntity.UpdatedAt = DateTime.Now;

            //isOrderRejected = false;
            //machine.Fire(Trigger.ActivateCompleted);
        }

        public void ActivateRejected() 
        {
            var result = behaviour.ActivateRejected(orders);
            //newOrder = result.InFlightOrder;
            InFlightOrder = result.InFlightOrder;
            mobileDataEntity.State = result.NewMobileState.ToString();
            //RejectInFlightOrder();
            mobileDataEntity.UpdatedAt = DateTime.Now;

            //isOrderRejected = true;
            //this.machine.Fire(Trigger.ActivateRejected);
        }

        public void ProcessingProvisionCompleted()
        {
            var result = behaviour.ProcessingProvisionCompleted(orders);
            InFlightOrder = result.InFlightOrder;
            mobileDataEntity.State = result.NewMobileState.ToString();
            //CompleteInFlightOrder();
            mobileDataEntity.UpdatedAt = DateTime.Now;
        }
        public void PortIn() => behaviour.PortIn();
        public void PortInCompleted() => behaviour.PortInCompleted();
        public void Suspend() => behaviour.Suspend();
        public void ReplaceSim() => behaviour.ReplaceSim();
        public void Resume() => behaviour.Resume();
        public void RequestPac() => behaviour.RequestPac();

        public void CeaseCompleted()
        {
            var result = behaviour.CeaseCompleted(orders);
            InFlightOrder = result.InFlightOrder;
            mobileDataEntity.State = result.NewMobileState.ToString();
        } 
        public void PortOutCompleted() => behaviour.PortOutCompleted();

        public void OrderProcessing()
        {
            if (InFlightOrder != null)
                InFlightOrder.Process();
        }

        public void OrderSent()
        {
            if (InFlightOrder != null)
                InFlightOrder.Send();
        }
    }
}
