using Stateless;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils.DomainDrivenDesign;
using Utils.Enums;

namespace MobileOrderer.Api.Domain
{
    public class Mobile : AggregateRoot
    {
        public enum State { New, ProcessingProvisioning, WaitingForActivation, ProcessingActivation, Live, ProcessingPortIn, Suspended, ProcessingCease, ProcessingPortOut, Ceased, PortedOut }
        public enum Trigger { Provision, ProcessingProvisioningCompleted, PortIn, Activate, ActivationCompleted, PortInCompleted, Cease, Suspend, ReplaceSim, Resume, RequestPac, CeaseCompleted, PortOutCompleted }

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
                .Permit(Trigger.Provision, State.ProcessingProvisioning);
            machine.Configure(State.ProcessingProvisioning)
                .Permit(Trigger.ProcessingProvisioningCompleted, State.WaitingForActivation)
                .OnEntry(() => {
                    this.mobileDataEntity.State = enumConverter.ToName<State>(this.CurrentState);
                })
                .OnExit(() =>
                {
                    this.CompleteInFlightOrder();
                });
            machine.Configure(State.WaitingForActivation)
                .Permit(Trigger.Activate, State.ProcessingActivation)
                .OnEntry(() =>
                {
                    this.mobileDataEntity.State = enumConverter.ToName<State>(this.CurrentState);
                });
            machine.Configure(State.ProcessingActivation)
                .Permit(Trigger.ActivationCompleted, State.Live)
                .OnEntry(() => {
                    this.mobileDataEntity.State = enumConverter.ToName<State>(State.ProcessingActivation);
                    this.CreateNewOrder();
                })
                .OnExit(() => {
                    this.CompleteInFlightOrder();
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
        public void ProcessingProvisioningCompleted() => this.machine.Fire(Trigger.ProcessingProvisioningCompleted);
        public void ActivateCompleted() => this.machine.Fire(Trigger.ActivationCompleted);
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

        private void CreateNewOrder()
        {
            this.InFlightOrder = newOrder;
            this.mobileDataEntity.AddOrder(newOrder.GetDataEntity());
        }
    }
}
