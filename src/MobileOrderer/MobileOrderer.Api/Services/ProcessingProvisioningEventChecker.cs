using MobileOrderer.Api.Data;

namespace MobileOrderer.Api.Services
{
    public class ProcessingProvisioningEventChecker : IMobileEventsChecker
    {
        private readonly IGetProcessingProvisioningMobilesQuery getNewMobilesQuery;
        private readonly IMobileCommand command;

        public ProcessingProvisioningEventChecker(
            IGetProcessingProvisioningMobilesQuery getProcessingProvisioningMobilesQuery,
            IMobileCommand command
            )
        {
            this.command = command;
            this.getNewMobilesQuery = getProcessingProvisioningMobilesQuery;
        }

        public void Check()
        {
            var newMobiles = this.getNewMobilesQuery.Get();

            foreach (var newMobile in newMobiles)
            {
                this.command.Execute(newMobile);
            }
        }
    }
}
