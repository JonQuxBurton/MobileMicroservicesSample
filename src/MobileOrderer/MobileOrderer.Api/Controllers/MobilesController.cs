using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MobileOrderer.Api.Domain;
using MobileOrderer.Api.Resources;
using System;
using Utils.DomainDrivenDesign;
using Utils.Enums;
using Utils.Guids;

namespace MobileOrderer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MobilesController : ControllerBase
    {
        private readonly ILogger<MobilesController> logger;
        private readonly IRepository<Mobile> mobileRepository;
        private readonly IGuidCreator guidCreator;
        private readonly IMonitoring monitoring;

        public MobilesController(ILogger<MobilesController> logger, IRepository<Mobile> mobileRepository, IGuidCreator guidCreator, IMonitoring monitoring)
        {
            this.logger = logger;
            this.mobileRepository = mobileRepository;
            this.guidCreator = guidCreator;
            this.monitoring = monitoring;
        }

        [HttpGet("{id}")]
        public ActionResult<Mobile> Get(Guid id)
        {
            var mobile = this.mobileRepository.GetById(id);

            if (mobile == null)
                return NotFound();

            return new OkObjectResult(mobile);
        }

        [HttpPost("{id}/activate")]
        public IActionResult Activate(Guid id, [FromBody] ActivateRequest activateRequest)
        {
            var mobile = this.mobileRepository.GetById(id);

            if (mobile == null)
            {
                logger.LogWarning("Attempt to Activate an unknown Mobile - MobileId: {MobileId}", id);
                return NotFound();
            }

            var newStateName = Order.State.New.ToString();
            var orderType = Order.OrderType.Activate.ToString();
            var dataEntity = new OrderDataEntity()
            {
                GlobalId = this.guidCreator.Create(),
                ActivationCode = activateRequest.ActivationCode,
                State = newStateName,
                Type = orderType
            };
            var inFlightOrder = new Order(dataEntity);

            mobile.Activate(inFlightOrder);
            mobileRepository.Update(mobile);

            monitoring.Activate();

            return new OkObjectResult(new OrderResource
            {
                GlobalId = dataEntity.GlobalId,
                Name = dataEntity.Name,
                ContactPhoneNumber = dataEntity.ContactPhoneNumber,
                State = dataEntity.State,
                Type = dataEntity.Type,
                CreatedAt = dataEntity.CreatedAt,
                UpdatedAt = dataEntity.UpdatedAt
            });
        }

        [HttpDelete("{id}")]
        public IActionResult Cease(Guid id)
        {
            var mobile = this.mobileRepository.GetById(id);

            if (mobile == null)
            {
                logger.LogWarning("Attempt to Cease an unknown Mobile - MobileId: {MobileId}", id);
                return NotFound();
            }

            var newStateName = Order.State.New.ToString();
            var orderType = Order.OrderType.Cease.ToString();
            var dataEntity = new OrderDataEntity()
            {
                GlobalId = this.guidCreator.Create(),
                State = newStateName,
                Type = orderType
            };
            var inFlightOrder = new Order(dataEntity); 
            mobile.Cease(inFlightOrder);
            mobileRepository.Update(mobile);

            monitoring.Cease();

            return Accepted();
        }
    }
}