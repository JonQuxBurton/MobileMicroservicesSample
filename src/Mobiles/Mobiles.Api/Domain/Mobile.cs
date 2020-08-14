using Stateless;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils.DomainDrivenDesign;
using Utils.Enums;

namespace Mobiles.Api.Domain
{
    public class Mobile : AggregateRoot
    {
        public enum State { New, ProcessingProvision, WaitingForActivate, ProcessingActivate, Live, ProcessingPortIn, Suspended, ProcessingCease, ProcessingPortOut, Ceased, PortedOut }
        public enum Trigger { Provision, ProcessingProvisionCompleted, PortIn, Activate, ActivateCompleted, ActivateRejected, PortInCompleted, Cease, Suspend, ReplaceSim, Resume, RequestPac, CeaseCompleted, PortOutCompleted }

        public override int Id { get => this.mobileDataEntity.Id; protected set => base.Id = value; }
        public Guid GlobalId => this.mobileDataEntity.GlobalId;
        public Guid CustomerId => this.mobileDataEntity.CustomerId;
        public PhoneNumber PhoneNumber => new PhoneNumber(this.mobileDataEntity.PhoneNumber);
        public DateTime? CreatedAt => this.mobileDataEntity.CreatedAt;
        public DateTime? UpdatedAt => this.mobileDataEntity.UpdatedAt;
        public Order InFlightOrder { get; private set; }
        public IEnumerable<Order> OrderHistory => this.orderHistory;
        public State CurrentState => machine.State;

        private readonly StateMachine<State, Trigger> machine;
        private readonly MobileDataEntity mobileDataEntity;
        private readonly List<Order> orderHistory;
        private Order newOrder;
        private bool isOrderRejected = false;

        public Mobile(MobileDataEntity mobileDataEntity, Order inFlightOrder, IEnumerable<Order> orderHistory = null)
        {
            this.mobileDataEntity = mobileDataEntity;
            this.InFlightOrder = inFlightOrder;

            var enumConverter = new EnumConverter();
            var initialState = enumConverter.ToEnum<State>(mobileDataEntity.State);

            if (orderHistory == null)
                this.orderHistory = new List<Order>();
            else
                this.orderHistory = orderHistory.OrderByDescending(x => x.CreatedAt).ToList();

            machine = new StateMachine<State, Trigger>(initialState);

            machine.Configure(State.New)
                .Permit(Trigger.Provision, State.ProcessingProvision);
            machine.Configure(State.ProcessingProvision)
                .Permit(Trigger.ProcessingProvisionCompleted, State.WaitingForActivate)
                .OnEntry(() => {
                    this.mobileDataEntity.State = enumConverter.ToName<State>(this.CurrentState);
                })
                .OnExit(() =>
                {
                    this.CompleteInFlightOrder();
                });
            machine.Configure(State.WaitingForActivate)
                .Permit(Trigger.Activate, State.ProcessingActivate)
                .OnEntry(() =>
                {
                    this.mobileDataEntity.State = enumConverter.ToName<State>(this.CurrentState);
                });
            machine.Configure(State.ProcessingActivate)
                .Permit(Trigger.ActivateCompleted, State.Live)
                .Permit(Trigger.ActivateRejected, State.WaitingForActivate)
                .OnEntry(() => {
                    this.mobileDataEntity.State = State.ProcessingActivate.ToString();
                    this.CreateNewOrder();
                })
                .OnExit(() => {
                    if (isOrderRejected)
                        RejectInFlightOrder();
                    else
                        CompleteInFlightOrder();
                });
            machine.Configure(State.Live)
                .Permit(Trigger.Cease, State.ProcessingCease)
                .OnEntry(() =>
                {
                    this.mobileDataEntity.State = enumConverter.ToName<State>(State.Live);
                });
            machine.Configure(State.ProcessingCease)
                .Permit(Trigger.CeaseCompleted, State.Ceased)
                .OnEntry(() =>
                {
                    this.mobileDataEntity.State = enumConverter.ToName<State>(State.ProcessingCease);
                    this.CreateNewOrder();
                })
                .OnExit(() =>
                {
                    this.CompleteInFlightOrder();
                });
            machine.Configure(State.Ceased)
                .OnEntry(() =>
                {
                    this.mobileDataEntity.State = enumConverter.ToName<State>(State.Ceased);
                });
            machine.Configure(State.New).Permit(Trigger.PortIn, State.ProcessingPortIn);
            machine.Configure(State.ProcessingPortIn).Permit(Trigger.PortInCompleted, State.Live);
            machine.Configure(State.Live).Permit(Trigger.Suspend, State.Suspended);
            machine.Configure(State.Suspended).Permit(Trigger.Resume, State.Live);
            machine.Configure(State.Live).Permit(Trigger.ReplaceSim, State.Suspended);
            machine.Configure(State.Live).Permit(Trigger.RequestPac, State.ProcessingPortOut);
            machine.Configure(State.ProcessingPortOut).Permit(Trigger.PortOutCompleted, State.PortedOut);
        }

        public MobileDataEntity GetDataEntity()
        {
            return this.mobileDataEntity;
        }

        public void Provision(Order order)
        {
            this.newOrder = order;
            this.machine.Fire(Trigger.Provision); 
        }
        public void Activate(Order order)
        {
            this.newOrder = order;
            this.machine.Fire(Trigger.Activate);
        }
        public void Cease(Order order)
        {
            this.newOrder = order;
            this.machine.Fire(Trigger.Cease);
        }

        public void ActivateCompleted() 
        {
            isOrderRejected = false;
            machine.Fire(Trigger.ActivateCompleted);
        }

        public void ActivateRejected() 
        {
            isOrderRejected = true;
            this.machine.Fire(Trigger.ActivateRejected);
        }
        public void ProcessingProvisionCompleted() => this.machine.Fire(Trigger.ProcessingProvisionCompleted);
        public void PortIn() => this.machine.Fire(Trigger.PortIn);
        public void PortInCompleted() => this.machine.Fire(Trigger.PortInCompleted);
        public void Suspend() => this.machine.Fire(Trigger.Suspend);
        public void ReplaceSim() => this.machine.Fire(Trigger.ReplaceSim);
        public void Resume() => this.machine.Fire(Trigger.ReplaceSim);
        public void RequestPac() => this.machine.Fire(Trigger.RequestPac);
        public void CeaseCompleted() => this.machine.Fire(Trigger.CeaseCompleted);
        public void PortOutCompleted() => this.machine.Fire(Trigger.PortOutCompleted);

        public void OrderProcessing()
        {
            if (this.InFlightOrder != null)
                this.InFlightOrder.Process();
        }

        public void OrderSent()
        {
            if (this.InFlightOrder != null)
                this.InFlightOrder.Send();
        }

        private void CompleteInFlightOrder()
        {
            if (this.InFlightOrder != null)
            {
                this.InFlightOrder.Complete();
                this.orderHistory.Add(this.InFlightOrder);
                this.InFlightOrder = null;
            }
        }

        private void RejectInFlightOrder()
        {
            if (this.InFlightOrder != null)
            {
                this.InFlightOrder.Reject();
                this.orderHistory.Add(this.InFlightOrder);
                this.InFlightOrder = null;
            }
        }

        private void CreateNewOrder()
        {
            this.InFlightOrder = newOrder;
            this.mobileDataEntity.AddOrder(newOrder.GetDataEntity());
        }
    }
}
