using Stateless;
using Utils.DateTimes;
using Utils.Enums;
using static Mobiles.Api.Domain.Mobile;

namespace Mobiles.Api.Domain
{
    public class MobileBehaviour
    {
        private readonly IDateTimeCreator dateTimeCreator;
        private readonly StateMachine<MobileState, Trigger> machine;
        private bool isOrderRejected;

        public MobileBehaviour(IDateTimeCreator dateTimeCreator, MobileDataEntity mobileDataEntity)
        {
            this.dateTimeCreator = dateTimeCreator;
            var enumConverter = new EnumConverter();
            var initialState = enumConverter.ToEnum<MobileState>(mobileDataEntity.State);

            machine = new StateMachine<MobileState, Trigger>(initialState);

            machine.Configure(MobileState.New)
                .Permit(Trigger.Provision, MobileState.ProcessingProvision);
            machine.Configure(MobileState.ProcessingProvision)
                .Permit(Trigger.ProcessingProvisionCompleted, MobileState.WaitingForActivate)
                .OnEntry(() => { CreateNewOrder(); })
                .OnExit(() => { CompleteInProgressOrder(); });
            machine.Configure(MobileState.WaitingForActivate)
                .Permit(Trigger.Activate, MobileState.ProcessingActivate);
            machine.Configure(MobileState.ProcessingActivate)
                .Permit(Trigger.ActivateCompleted, MobileState.Live)
                .Permit(Trigger.ActivateRejected, MobileState.WaitingForActivate)
                .OnEntry(() => { CreateNewOrder(); })
                .OnExit(() =>
                {
                    if (isOrderRejected)
                        RejectInProgressOrder();
                    else
                        CompleteInProgressOrder();
                });
            machine.Configure(MobileState.Live)
                .Permit(Trigger.Cease, MobileState.ProcessingCease);
            machine.Configure(MobileState.ProcessingCease)
                .Permit(Trigger.CeaseCompleted, MobileState.Ceased)
                .OnEntry(() => { CreateNewOrder(); })
                .OnExit(() => { CompleteInProgressOrder(); });
            machine.Configure(MobileState.Ceased);
            machine.Configure(MobileState.New).Permit(Trigger.PortIn, MobileState.ProcessingPortIn);
            machine.Configure(MobileState.ProcessingPortIn).Permit(Trigger.PortInCompleted, MobileState.Live);
            machine.Configure(MobileState.Live).Permit(Trigger.Suspend, MobileState.Suspended);
            machine.Configure(MobileState.Suspended).Permit(Trigger.Resume, MobileState.Live);
            machine.Configure(MobileState.Live).Permit(Trigger.ReplaceSim, MobileState.Suspended);
            machine.Configure(MobileState.Live).Permit(Trigger.RequestPac, MobileState.ProcessingPortOut);
            machine.Configure(MobileState.ProcessingPortOut).Permit(Trigger.PortOutCompleted, MobileState.PortedOut);
        }

        private MobileDataEntity MobileDataEntity { get; set; }
        private Order InProgressOrder { get; set; }

        public void Provision(MobileDataEntity mobileDataEntity, Order inProgressOrder)
        {
            FireTrigger(Trigger.Provision, mobileDataEntity, inProgressOrder);
        }

        public void Activate(MobileDataEntity mobileDataEntity, Order inProgressOrder)
        {
            FireTrigger(Trigger.Activate, mobileDataEntity, inProgressOrder);
        }

        public void Cease(MobileDataEntity mobileDataEntity, Order inProgressOrder)
        {
            FireTrigger(Trigger.Cease, mobileDataEntity, inProgressOrder);
        }

        public void ActivateCompleted(MobileDataEntity mobileDataEntity, Order inProgressOrder)
        {
            isOrderRejected = false;
            FireTrigger(Trigger.ActivateCompleted, mobileDataEntity, inProgressOrder);
        }

        public void ActivateRejected(MobileDataEntity mobileDataEntity, Order inProgressOrder)
        {
            isOrderRejected = true;
            FireTrigger(Trigger.ActivateRejected, mobileDataEntity, inProgressOrder);
        }

        public void ProcessingProvisionCompleted(MobileDataEntity mobileDataEntity, Order inProgressOrder)
        {
            FireTrigger(Trigger.ProcessingProvisionCompleted, mobileDataEntity, inProgressOrder);
        }

        public void PortIn()
        {
            machine.Fire(Trigger.PortIn);
        }

        public void PortInCompleted()
        {
            machine.Fire(Trigger.PortInCompleted);
        }

        public void Suspend()
        {
            machine.Fire(Trigger.Suspend);
        }

        public void ReplaceSim()
        {
            machine.Fire(Trigger.ReplaceSim);
        }

        public void Resume()
        {
            machine.Fire(Trigger.ReplaceSim);
        }

        public void RequestPac()
        {
            machine.Fire(Trigger.RequestPac);
        }

        public void CeaseCompleted(MobileDataEntity mobileDataEntity, Order inProgressOrder)
        {
            FireTrigger(Trigger.CeaseCompleted, mobileDataEntity, inProgressOrder);
        }

        public void PortOutCompleted()
        {
            machine.Fire(Trigger.PortOutCompleted);
        }

        private void FireTrigger(Trigger trigger, MobileDataEntity mobileDataEntity, Order inProgressOrder)
        {
            MobileDataEntity = mobileDataEntity;
            InProgressOrder = inProgressOrder;
            machine.Fire(trigger);
            mobileDataEntity.State = machine.State.ToString();
            mobileDataEntity.UpdatedAt = dateTimeCreator.Create();
        }

        private void CompleteInProgressOrder()
        {
            InProgressOrder?.Complete();
        }

        private void RejectInProgressOrder()
        {
            InProgressOrder?.Reject();
        }

        private void CreateNewOrder()
        {
            if (InProgressOrder != null)
                MobileDataEntity.AddOrder(InProgressOrder.GetDataEntity());
        }
    }
}