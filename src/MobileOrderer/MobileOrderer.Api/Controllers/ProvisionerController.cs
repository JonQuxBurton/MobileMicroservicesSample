using Microsoft.AspNetCore.Mvc;
using MobileOrderer.Api.Domain;
using MobileOrderer.Api.Resources;
using System;
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
        public IActionResult Post([FromBody] OrderToAdd orderToAdd)
        {
            var mobileBuilder = new MobileBuilder(this.guidCreator.Create())
                            .AddInFlightOrder(orderToAdd, this.guidCreator.Create());
            this.mobileRepository.Add(mobileBuilder.Build());
            
            return Ok();
        }
    }
}