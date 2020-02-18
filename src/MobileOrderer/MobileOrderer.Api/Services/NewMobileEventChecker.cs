using MobileOrderer.Api.Data;

namespace MobileOrderer.Api.Services
{
    public class NewMobileEventChecker : IMobileEventsChecker
    {
        private readonly IGetNewMobilesQuery getNewMobilesQuery;
        private readonly IMobileCommand command;

        public NewMobileEventChecker(
            IGetNewMobilesQuery getNewMobilesQuery,
            IMobileCommand command
            )
        {
            this.command = command;
            this.getNewMobilesQuery = getNewMobilesQuery;
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
