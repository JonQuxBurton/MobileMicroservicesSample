using Stateless;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MobileOrderer.Api.Domain
{
    public class Mobile : AggregateRoot
    {
        public enum State { New, PendingLive, Live, PendingPortIn, Suspended, PendingCease, PendingPortOut, Ceased, PortedOut }
        public enum Trigger { Provision, PortIn, Activate, PortInCompleted, Cease, Suspend, ReplaceSim, Resume, RequestPac, CeaseCompleted, PortOutCompleted }

        public override int Id { get; protected set; }
        public Guid GlobalId { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public MobileOrder InFlightOrder { get; private set; }
        public IEnumerable<MobileOrder> OrderHistory { get; private set; }
        public State CurrentState => machine.State;

        private readonly StateMachine<State, Trigger> machine;

        public Mobile(State initialState, Guid globalId, int id, MobileOrder inFlightOrder, IEnumerable<MobileOrder> orderHistory)
        {
            GlobalId = globalId;
            Id = id;
            InFlightOrder = inFlightOrder;
            if (orderHistory == null)
                OrderHistory = Enumerable.Empty<MobileOrder>();
            else
                OrderHistory = orderHistory.OrderByDescending(x => x.CreatedAt).ToList();

            machine = new StateMachine<State, Trigger>(initialState);

            machine.Configure(State.New).Permit(Trigger.Provision, State.PendingLive);
            machine.Configure(State.PendingLive)
                .OnEntry(() => {
                    if (this.InFlightOrder != null)
                        this.InFlightOrder.Process();
                })
                .Permit(Trigger.Activate, State.Live);
            machine.Configure(State.New).Permit(Trigger.PortIn, State.PendingPortIn);
            machine.Configure(State.PendingPortIn).Permit(Trigger.PortInCompleted, State.Live);
            machine.Configure(State.Live).Permit(Trigger.Suspend, State.Suspended);
            machine.Configure(State.Suspended).Permit(Trigger.Resume, State.Live);
            machine.Configure(State.Live).Permit(Trigger.ReplaceSim, State.Suspended);
            machine.Configure(State.Live).Permit(Trigger.RequestPac, State.PendingPortOut);
            machine.Configure(State.Live).Permit(Trigger.Cease, State.PendingCease);
            machine.Configure(State.PendingCease).Permit(Trigger.CeaseCompleted, State.Ceased);
            machine.Configure(State.PendingPortOut).Permit(Trigger.PortOutCompleted, State.PortedOut);
        }

        public void Provision() => this.machine.Fire(Trigger.Provision);
        public void Activate() => this.machine.Fire(Trigger.Activate);
        public void PortIn() => this.machine.Fire(Trigger.PortIn);
        public void PortInCompleted() => this.machine.Fire(Trigger.PortInCompleted);
        public void Cease() => this.machine.Fire(Trigger.Cease);
        public void Suspend() => this.machine.Fire(Trigger.Suspend);
        public void ReplaceSim() => this.machine.Fire(Trigger.ReplaceSim);
        public void Resume() => this.machine.Fire(Trigger.ReplaceSim);
        public void RequestPac() => this.machine.Fire(Trigger.RequestPac);
        public void CeaseCompleted() => this.machine.Fire(Trigger.CeaseCompleted);
        public void PortOutCompleted() => this.machine.Fire(Trigger.PortOutCompleted);
    }
}
