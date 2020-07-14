using MobileOrderer.Api.Data;

namespace MobileOrderer.Api.Services
{
    public class CeaseRequestedEventChecker : IMobileEventsChecker
    {
        private readonly IGetNewCeasesQuery getMobilesQuery;
        private readonly IMobileCommand command;

        public CeaseRequestedEventChecker(
            IGetNewCeasesQuery getCeasesMobilesQuery,
            IMobileCommand command
            )
        {
            this.command = command;
            this.getMobilesQuery = getCeasesMobilesQuery;
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
