using Microsoft.Extensions.Logging;
using Mobiles.Api.Data;
using Mobiles.Api.Domain;
using Utils.DomainDrivenDesign;

namespace Mobiles.Api.Services
{
    public class MobileProvisionRequestedEventChecker : IMobileEventsChecker
    {
        private readonly ILogger<MobileProvisionRequestedEventChecker> logger;
        private readonly IGetNeProvisionsQuery getNewMobilesQuery;
        private readonly IRepository<Mobile> mobileRepository;

        public MobileProvisionRequestedEventChecker(
            ILogger<MobileProvisionRequestedEventChecker> logger,
            IGetNeProvisionsQuery getNewMobilesQuery,
            IRepository<Mobile> mobileRepository
            )
        {
            this.logger = logger;
            this.getNewMobilesQuery = getNewMobilesQuery;
            this.mobileRepository = mobileRepository;
        }

        public void Check()
        {
            var newMobiles = this.getNewMobilesQuery.Get();

            foreach (var newMobile in newMobiles)
            {
                Provision(newMobile);
            }
        }

        private void Provision(Mobile mobile)
        {
            mobile.Provision(mobile.InFlightOrder);

            mobileRepository.Update(mobile);
        }
    }
}
