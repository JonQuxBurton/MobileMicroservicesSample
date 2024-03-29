﻿using Stateless;
using System;
using Utils.DomainDrivenDesign;
using Utils.Enums;

namespace Mobiles.Api.Domain
{
    public class Order : Entity
    {
        public enum State { New, Processing, Sent, Completed, Failed, Cancelled, Rejected }
        public enum Trigger { Process, Send, Complete, Fail, Cancel, Reject }
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
                    this.orderDataEntity.State = State.Processing.ToString();
                })
                .Permit(Trigger.Send, State.Sent);
            machine.Configure(State.Sent)
                .OnEntry(() => {
                    this.orderDataEntity.State = State.Sent.ToString();
                })
                .Permit(Trigger.Complete, State.Completed)
                .Permit(Trigger.Reject, State.Rejected);
            machine.Configure(State.Completed)
                .OnEntry(() =>
                {
                    this.orderDataEntity.State = State.Completed.ToString();
                });
            machine.Configure(State.Rejected)
                .OnEntry(() =>
                {
                    this.orderDataEntity.State = State.Rejected.ToString();
                });
        }

        public Guid GlobalId => orderDataEntity.GlobalId;
        public int MobileId => orderDataEntity.MobileId;
        public string Name => orderDataEntity.Name;
        public string ContactPhoneNumber => orderDataEntity.ContactPhoneNumber;
        public State CurrentState => machine.State;

        public OrderType Type
        {
            get
            {
                if (orderDataEntity.Type == null)
                    return OrderType.Provision;

                return enumConverter.ToEnum<OrderType>(orderDataEntity.Type);
            }
        }
        public DateTime CreatedAt => orderDataEntity.CreatedAt;
        public DateTime? UpdatedAt => orderDataEntity.UpdatedAt;
        public string ActivationCode => orderDataEntity.ActivationCode;

        public OrderDataEntity GetDataEntity()
        {
            return orderDataEntity;
        }

        public void Process()
        {
            machine.Fire(Trigger.Process);
        }
        
        public void Send()
        {
            machine.Fire(Trigger.Send);
        }

        public void Complete()
        {
            machine.Fire(Trigger.Complete);
        }

        public void Reject()
        {
            machine.Fire(Trigger.Reject);
        }
    }
}
