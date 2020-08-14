using Stateless;
using System;
using Utils.DomainDrivenDesign;
using Utils.Enums;

namespace Mobiles.Api.Domain
{
    public class Order : Entity
    {
        public enum State { New, Processing, Sent, Completed, Failed, Cancelled, Rejected
        }
        public enum Trigger { Process, Send, Complete, Fail, Cancel, Reject
        }
        public enum OrderType { Provision, Activate, Cease }

        private readonly StateMachine<State, Trigger> machine;
        private readonly OrderDataEntity orderDataEntity;
        private readonly EnumConverter enumConverter;

        public Order(OrderDataEntity orderDataEntity)
        {
            this.orderDataEntity = orderDataEntity;

            enumConverter = new EnumConverter();
            var initialState = enumConverter.ToEnum<State>(orderDataEntity.State);
            machine = new StateMachine<State, Trigger>(initialState);

            machine.Configure(State.New).Permit(Trigger.Process, State.Processing);
            machine.Configure(State.Processing)
                .OnEntry(() => {
                    this.orderDataEntity.State = enumConverter.ToName<State>(State.Processing);
                })
                .Permit(Trigger.Send, State.Sent);
            machine.Configure(State.Sent)
                .OnEntry(() => {
                    this.orderDataEntity.State = enumConverter.ToName<State>(State.Sent);
                })
                .Permit(Trigger.Complete, State.Completed)
                .Permit(Trigger.Reject, State.Rejected);
            machine.Configure(State.Completed)
                .OnEntry(() =>
                {
                    this.orderDataEntity.State = enumConverter.ToName<State>(State.Completed);
                });
        }

        public Guid GlobalId => this.orderDataEntity.GlobalId;
        public int MobileId => this.orderDataEntity.MobileId;
        public string Name => this.orderDataEntity.Name;
        public string ContactPhoneNumber => this.orderDataEntity.ContactPhoneNumber;
        public State CurrentState => machine.State;
        public OrderType Type => enumConverter.ToEnum<OrderType>(this.orderDataEntity.Type);
        public DateTime? CreatedAt => this.orderDataEntity.CreatedAt;
        public DateTime? UpdatedAt => this.orderDataEntity.UpdatedAt;
        public string ActivationCode => this.orderDataEntity.ActivationCode;

        public OrderDataEntity GetDataEntity()
        {
            return this.orderDataEntity;
        }

        public void Process()
        {
            this.machine.Fire(Trigger.Process);
        }
        
        public void Send()
        {
            this.machine.Fire(Trigger.Send);
        }

        public void Complete()
        {
            this.machine.Fire(Trigger.Complete);
        }

        public void Reject()
        {
            this.machine.Fire(Trigger.Reject);
        }
    }
}
