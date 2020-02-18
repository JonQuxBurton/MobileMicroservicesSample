using MobileOrderer.Api.Domain;
using Utils.DomainDrivenDesign;

namespace MobileOrderer.Api.Services
{
    public class ProvisionCommand : IMobileCommand
    {
        private readonly IRepository<Mobile> mobileRepository;

        public ProvisionCommand(IRepository<Mobile> mobileRepository)
        {
            this.mobileRepository = mobileRepository;
        }

        public void Execute(Mobile mobile)
        {
            mobile.Provision(mobile.InFlightOrder);
            mobileRepository.Update(mobile);
        }
    }
}
