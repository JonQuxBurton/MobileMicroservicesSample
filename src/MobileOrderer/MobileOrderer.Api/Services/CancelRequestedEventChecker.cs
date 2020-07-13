using MobileOrderer.Api.Data;

namespace MobileOrderer.Api.Services
{
    public class CancelRequestedEventChecker : IMobileEventsChecker
    {
        private readonly IGetNewCancelsQuery getMobilesQuery;
        private readonly IMobileCommand command;

        public CancelRequestedEventChecker(
            IGetNewCancelsQuery getCancelMobilesQuery,
            IMobileCommand command
            )
        {
            this.command = command;
            this.getMobilesQuery = getCancelMobilesQuery;
        }

        public void Check()
        {
            var newMobiles = this.getMobilesQuery.Get();

            foreach (var newMobile in newMobiles)
            {
                this.command.Execute(newMobile);
            }
        }
    }
}
