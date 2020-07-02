using Microsoft.AspNetCore.Mvc;
using MobileOrderer.Api.Domain;
using MobileOrderer.Api.Resources;
using Utils.DomainDrivenDesign;
using Utils.Guids;

namespace MobileOrderer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProvisionerController : ControllerBase
    {
        private readonly IRepository<Mobile> mobileRepository;
        private readonly IGuidCreator guidCreator;        

        public ProvisionerController(IRepository<Mobile> mobileRepository, IGuidCreator guidCreator)
        {
            this.mobileRepository = mobileRepository;
            this.guidCreator = guidCreator;
        }

        [HttpGet("status")]
        public IActionResult Status()
        {
            return new OkResult();
        }

        [HttpPost]
        public ActionResult<MobileResource> Post([FromBody] OrderToAdd orderToAdd)
        {
            var mobile = new MobileWhenNewBuilder(this.guidCreator.Create())
                            .AddInFlightOrder(orderToAdd, this.guidCreator.Create())
                            .Build();
            this.mobileRepository.Add(mobile);

            return new OkObjectResult(new MobileResource 
            { 
                Id = mobile.Id,
                GlobalId = mobile.GlobalId,
                CreatedAt = mobile.CreatedAt,
                UpdatedAt = mobile.UpdatedAt
            });
        }
    }
}