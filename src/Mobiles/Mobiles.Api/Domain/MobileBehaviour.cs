using Stateless;
using Utils.Enums;
using static Mobiles.Api.Domain.Mobile;

namespace Mobiles.Api.Domain
{
    public class MobileTransition
    {
        public MobileTransition(MobileState mobileState, string action)
        {
            MobileState = mobileState;
            Action = action;
        }

        public MobileState MobileState { get; private set; }
        public string Action { get; private set; }
    }

    public class MobileBehaviour
    {
        private readonly StateMachine<MobileState, Trigger> machine;
        private bool isOrderRejected = false;
        private string nextAction;

        public MobileBehaviour(MobileDataEntity mobileDataEntity)
        {
            var enumConverter = new EnumConverter();
            var initialState = enumConverter.ToEnum<MobileState>(mobileDataEntity.State);

            machine = new StateMachine<MobileState, Trigger>(initialState);

            machine.Configure(MobileState.New)
                .Permit(Trigger.Provision, MobileState.ProcessingProvision);
            machine.Configure(MobileState.ProcessingProvision)
                .Permit(Trigger.ProcessingProvisionCompleted, MobileState.WaitingForActivate)
                .OnExit(() =>
                {
                    CompleteInFlightOrder();
                });
            machine.Configure(MobileState.WaitingForActivate)
                .Permit(Trigger.Activate, MobileState.ProcessingActivate);
            machine.Configure(MobileState.ProcessingActivate)
                .Permit(Trigger.ActivateCompleted, MobileState.Live)
                .Permit(Trigger.ActivateRejected, MobileState.WaitingForActivate)
                .OnEntry(() => {
                    CreateNewOrder();
                })
                .OnExit(() => {
                    if (isOrderRejected)
                        RejectInFlightOrder();
                    else
                        CompleteInFlightOrder();
                });
            machine.Configure(MobileState.Live)
                .Permit(Trigger.Cease, MobileState.ProcessingCease);
            machine.Configure(MobileState.ProcessingCease)
                .Permit(Trigger.CeaseCompleted, MobileState.Ceased)
                .OnEntry(() =>
                {
                    CreateNewOrder();
                })
                .OnExit(() =>
                {
                    CompleteInFlightOrder();
                });
            machine.Configure(MobileState.Ceased);
            machine.Configure(MobileState.New).Permit(Trigger.PortIn, MobileState.ProcessingPortIn);
            machine.Configure(MobileState.ProcessingPortIn).Permit(Trigger.PortInCompleted, MobileState.Live);
            machine.Configure(MobileState.Live).Permit(Trigger.Suspend, MobileState.Suspended);
            machine.Configure(MobileState.Suspended).Permit(Trigger.Resume, MobileState.Live);
            machine.Configure(MobileState.Live).Permit(Trigger.ReplaceSim, MobileState.Suspended);
            machine.Configure(MobileState.Live).Permit(Trigger.RequestPac, MobileState.ProcessingPortOut);
            machine.Configure(MobileState.ProcessingPortOut).Permit(Trigger.PortOutCompleted, MobileState.PortedOut);

        }

        public MobileTransition Provision()
        {
            nextAction = null;
            machine.Fire(Trigger.Provision);
            return new MobileTransition(machine.State, nextAction);
        }

        public MobileTransition Activate()
        {
            nextAction = null;
            machine.Fire(Trigger.Activate);
            return new MobileTransition(machine.State, nextAction);

        }
        public MobileTransition Cease()
        {
            nextAction = null;
            machine.Fire(Trigger.Cease);
            return new MobileTransition(machine.State, nextAction);
        }

        public MobileTransition ActivateCompleted()
        {
            nextAction = null;
            isOrderRejected = false;
            machine.Fire(Trigger.ActivateCompleted);
            return new MobileTransition(machine.State, nextAction);
        }

        public MobileTransition ActivateRejected()
        {
            nextAction = null;
            isOrderRejected = true;
            machine.Fire(Trigger.ActivateRejected);
            return new MobileTransition(machine.State, nextAction);
        }

        public MobileTransition ProcessingProvisionCompleted()
        {
            nextAction = null;
            machine.Fire(Trigger.ProcessingProvisionCompleted);
            return new MobileTransition(machine.State, nextAction);
        }

        public void PortIn() => machine.Fire(Trigger.PortIn);
        public void PortInCompleted() => machine.Fire(Trigger.PortInCompleted);
        public void Suspend() => machine.Fire(Trigger.Suspend);
        public void ReplaceSim() => machine.Fire(Trigger.ReplaceSim);
        public void Resume() => machine.Fire(Trigger.ReplaceSim);
        public void RequestPac() => machine.Fire(Trigger.RequestPac);

        public MobileTransition CeaseCompleted()
        {
            nextAction = null;
            machine.Fire(Trigger.CeaseCompleted);
            return new MobileTransition(machine.State, nextAction);
        }
        public void PortOutCompleted() => machine.Fire(Trigger.PortOutCompleted);

        private void CompleteInFlightOrder()
        {
            nextAction = "CompleteInFlightOrder";
        }

        private void RejectInFlightOrder()
        {
            nextAction = "RejectInFlightOrder";
        }

        private void CreateNewOrder()
        {
            nextAction = "CreateNewOrder";
        }
    }
}
