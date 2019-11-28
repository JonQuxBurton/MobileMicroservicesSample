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
        public IActionResult Post([FromBody] MobileOrderToAdd orderToAdd)
        {
            var mobileBuilder = new MobileBuilder(Mobile.State.New, this.guidCreator.Create());
            mobileBuilder.AddInFlightOrder(orderToAdd, this.guidCreator.Create());
            this.mobileRepository.Save(mobileBuilder.Build());
            
            return Ok();
        }

        [HttpGet("{id}")]
        public ActionResult<Mobile> Get(Guid id)
        {
            var mobile = this.mobileRepository.GetById(id);

            if (mobile == null)
                return NotFound();

            return new OkObjectResult(mobile);
        }
    }
}