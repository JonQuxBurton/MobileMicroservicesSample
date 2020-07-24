﻿using MobileOrderer.Api.Data;
using MobileOrderer.Api.Domain;
using Utils.DomainDrivenDesign;

namespace MobileOrderer.Api.Services
{
    public class NewMobileEventChecker : IMobileEventsChecker
    {
        private readonly IGetNewMobilesQuery getNewMobilesQuery;
        private readonly IRepository<Mobile> mobileRepository;

        public NewMobileEventChecker(
            IGetNewMobilesQuery getNewMobilesQuery,
            IRepository<Mobile> mobileRepository
            )
        {
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
