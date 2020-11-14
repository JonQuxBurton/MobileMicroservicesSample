using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stateless;
using Utils.Enums;
using static Mobiles.Api.Domain.Mobile;

namespace Mobiles.Api.Domain
{
    public class MobileTransitionResult
    {
        public MobileTransitionResult(MobileState newMobileState, Order inFlightOrder)
        {
            NewMobileState = newMobileState;
            InFlightOrder = inFlightOrder;
        }

        public MobileState NewMobileState { get; }
        public Order InFlightOrder { get; }
    }

    public class MobileBehaviour
    {
        private readonly StateMachine<MobileState, Trigger> machine;
        private Order newOrder;
        private bool isOrderRejected = false;
        private MobileDataEntity mobileDataEntity;
        private List<Order> orderHistory;

        public MobileState CurrentMobileState => machine.State;
        public Order InFlightOrder { get; private set; }

        public MobileBehaviour(MobileDataEntity mobileDataEntity, Order inFlightOrder)
        {
            InFlightOrder = inFlightOrder;

            var enumConverter = new EnumConverter();
            var initialState = enumConverter.ToEnum<MobileState>(mobileDataEntity.State);

            machine = new StateMachine<MobileState, Trigger>(initialState);

            machine.Configure(MobileState.New)
                .Permit(Trigger.Provision, MobileState.ProcessingProvision);
            machine.Configure(MobileState.ProcessingProvision)
                .Permit(Trigger.ProcessingProvisionCompleted, MobileState.WaitingForActivate)
                .OnEntry(() =>
                {
                    //this.mobileDataEntity.UpdatedAt = DateTime.Now;
                    //this.mobileDataEntity.MobileState = enumConverter.ToName<MobileState>(this.NewMobileState);
                })
                .OnExit(() =>
                {
                    CompleteInFlightOrder();
                });
            machine.Configure(MobileState.WaitingForActivate)
                .Permit(Trigger.Activate, MobileState.ProcessingActivate)
                .OnEntry(() =>
                {
                    //this.mobileDataEntity.UpdatedAt = DateTime.Now;
                    //this.mobileDataEntity.MobileState = enumConverter.ToName<MobileState>(this.NewMobileState);
                });
            machine.Configure(MobileState.ProcessingActivate)
                .Permit(Trigger.ActivateCompleted, MobileState.Live)
                .Permit(Trigger.ActivateRejected, MobileState.WaitingForActivate)
                .OnEntry(() => {
                    //this.mobileDataEntity.UpdatedAt = DateTime.Now;
                    //this.mobileDataEntity.MobileState = MobileState.ProcessingActivate.ToString();
                    CreateNewOrder();
                })
                .OnExit(() => {
                    if (isOrderRejected)
                        RejectInFlightOrder();
                    else
                        CompleteInFlightOrder();
                });
            machine.Configure(MobileState.Live)
                .Permit(Trigger.Cease, MobileState.ProcessingCease)
                .OnEntry(() =>
                {
                    //this.mobileDataEntity.UpdatedAt = DateTime.Now;
                    //this.mobileDataEntity.MobileState = enumConverter.ToName<MobileState>(MobileState.Live);
                });
            machine.Configure(MobileState.ProcessingCease)
                .Permit(Trigger.CeaseCompleted, MobileState.Ceased)
                .OnEntry(() =>
                {
                    //this.mobileDataEntity.UpdatedAt = DateTime.Now;
                    //this.mobileDataEntity.MobileState = enumConverter.ToName<MobileState>(MobileState.ProcessingCease);
                    CreateNewOrder();
                })
                .OnExit(() =>
                {
                    CompleteInFlightOrder();
                });
            machine.Configure(MobileState.Ceased)
                .OnEntry(() =>
                {
                    //this.mobileDataEntity.UpdatedAt = DateTime.Now;
                    //this.mobileDataEntity.MobileState = enumConverter.ToName<MobileState>(MobileState.Ceased);
                });
            machine.Configure(MobileState.New).Permit(Trigger.PortIn, MobileState.ProcessingPortIn);
            machine.Configure(MobileState.ProcessingPortIn).Permit(Trigger.PortInCompleted, MobileState.Live);
            machine.Configure(MobileState.Live).Permit(Trigger.Suspend, MobileState.Suspended);
            machine.Configure(MobileState.Suspended).Permit(Trigger.Resume, MobileState.Live);
            machine.Configure(MobileState.Live).Permit(Trigger.ReplaceSim, MobileState.Suspended);
            machine.Configure(MobileState.Live).Permit(Trigger.RequestPac, MobileState.ProcessingPortOut);
            machine.Configure(MobileState.ProcessingPortOut).Permit(Trigger.PortOutCompleted, MobileState.PortedOut);

        }

        public MobileTransitionResult Provision(Order order)
        {
            newOrder = order;
            machine.Fire(Trigger.Provision);
            return new MobileTransitionResult(CurrentMobileState, order);
        }
        public MobileTransitionResult Activate(Order order, MobileDataEntity currentMobileDataEntity)
        {
            mobileDataEntity = currentMobileDataEntity;
            newOrder = order;
            machine.Fire(Trigger.Activate);
            return new MobileTransitionResult (CurrentMobileState, order);

        }
        public MobileTransitionResult Cease(Order order, MobileDataEntity currentMobileDataEntity)
        {
            mobileDataEntity = currentMobileDataEntity;
            newOrder = order;
            machine.Fire(Trigger.Cease);
            return new MobileTransitionResult(CurrentMobileState, order);
        }

        public MobileTransitionResult ActivateCompleted(List<Order> currentOrderHistory)
        {
            this.orderHistory = currentOrderHistory;
            isOrderRejected = false;
            machine.Fire(Trigger.ActivateCompleted);
            return new MobileTransitionResult(CurrentMobileState, null);
        }

        public MobileTransitionResult ActivateRejected(List<Order> currentOrderHistory)
        {
            this.orderHistory = currentOrderHistory;
            isOrderRejected = true;
            machine.Fire(Trigger.ActivateRejected);
            return new MobileTransitionResult(CurrentMobileState, null);
        }

        public MobileTransitionResult ProcessingProvisionCompleted(List<Order> currentOrderHistory)
        {
            this.orderHistory = currentOrderHistory;
            machine.Fire(Trigger.ProcessingProvisionCompleted);
            return new MobileTransitionResult(CurrentMobileState, InFlightOrder);
        }
        
        public void PortIn() => machine.Fire(Trigger.PortIn);
        public void PortInCompleted() => machine.Fire(Trigger.PortInCompleted);
        public void Suspend() => machine.Fire(Trigger.Suspend);
        public void ReplaceSim() => machine.Fire(Trigger.ReplaceSim);
        public void Resume() => machine.Fire(Trigger.ReplaceSim);
        public void RequestPac() => machine.Fire(Trigger.RequestPac);

        public MobileTransitionResult CeaseCompleted(List<Order> currentOrderHistory)
        {
            this.orderHistory = currentOrderHistory;
            machine.Fire(Trigger.CeaseCompleted);
            return new MobileTransitionResult(CurrentMobileState, InFlightOrder);
        } 
        public void PortOutCompleted() => machine.Fire(Trigger.PortOutCompleted);

        private void CompleteInFlightOrder()
        {
            if (InFlightOrder != null)
            {
                InFlightOrder.Complete();
                this.orderHistory.Add(this.InFlightOrder);
                InFlightOrder = null;
            }
        }

        private void RejectInFlightOrder()
        {
            if (InFlightOrder != null)
            {
                InFlightOrder.Reject();
                this.orderHistory.Add(this.InFlightOrder);
                InFlightOrder = null;
            }
        }

        private void CreateNewOrder()
        {
            InFlightOrder = newOrder;
            mobileDataEntity.AddOrder(newOrder.GetDataEntity());
        }
    }
}
