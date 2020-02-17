using Microsoft.AspNetCore.Mvc;
using MobileOrderer.Api.Domain;
using MobileOrderer.Api.Resources;
using System;
using Utils.DomainDrivenDesign;
using Utils.Enums;
using Utils.Guids;
using static MobileOrderer.Api.Domain.Mobile;
using static MobileOrderer.Api.Domain.Order;

namespace MobileOrderer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MobilesController : ControllerBase
    {
        private readonly IRepository<Mobile> mobileRepository;
        private readonly IGuidCreator guidCreator;        

        public MobilesController(IRepository<Mobile> mobileRepository, IGuidCreator guidCreator)
        {
            this.mobileRepository = mobileRepository;
            this.guidCreator = guidCreator;
        }

        [HttpPost("{id}/activate")]
        public IActionResult Post(Guid id, [FromBody] OrderToAdd orderToAdd)
        {
            var mobile = this.mobileRepository.GetById(id);

            if (mobile == null)
                return NotFound();

            var newStateName = new EnumConverter().ToName<Order.State>(Order.State.New);
            var dataEntity = new OrderDataEntity()
            {
                GlobalId = this.guidCreator.Create(),
                Name = orderToAdd.Name,
                ContactPhoneNumber = orderToAdd.ContactPhoneNumber,
                State = newStateName,
                Type = "Activate"
            };
            var inFlightOrder = new Order(dataEntity);

            mobile.Activate(inFlightOrder);
            this.mobileRepository.Update(mobile);

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