using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Mobiles.Api.Data;
using Mobiles.Api.Domain;
using Mobiles.Api.Resources;
using System;
using System.Linq;
using Utils.DomainDrivenDesign;
using Utils.Guids;

namespace Mobiles.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MobilesController : ControllerBase
    {
        private readonly ILogger<MobilesController> logger;
        private readonly IRepository<Mobile> mobileRepository;
        private readonly IGuidCreator guidCreator;
        private readonly IMonitoring monitoring;
        private readonly IGetNextMobileIdQuery getNextMobileIdQuery;

        public MobilesController(ILogger<MobilesController> logger, IRepository<Mobile> mobileRepository, IGuidCreator guidCreator, IMonitoring monitoring, IGetNextMobileIdQuery getNextMobileIdQuery)
        {
            this.logger = logger;
            this.mobileRepository = mobileRepository;
            this.guidCreator = guidCreator;
            this.monitoring = monitoring;
            this.getNextMobileIdQuery = getNextMobileIdQuery;
        }

        [HttpGet("availablePhoneNumbers")]
        public ActionResult<AvailablePhoneNumbersResource> GetAvailablePhoneNumbers()
        {
            var nextId = getNextMobileIdQuery.Get().ToString().PadLeft(3, '0');
            return new AvailablePhoneNumbersResource
            {
                PhoneNumbers = new[] { $"07{nextId}000{nextId}" }
            };
        }

        [HttpGet("{id}")]
        public ActionResult<Mobile> Get(Guid id)
        {
            var mobile = mobileRepository.GetById(id);

            if (mobile == null)
                return NotFound();

            OrderResource inFlightOrder = null;

            if (mobile.InFlightOrder != null)
            {
                inFlightOrder = new OrderResource
                {
                    GlobalId = mobile.InFlightOrder.GlobalId,
                    State = mobile.InFlightOrder.CurrentState.ToString(),
                    Type = mobile.InFlightOrder.Type.ToString(),
                };
            }

            return new OkObjectResult(new MobileResource
            {
                Id = mobile.Id,
                GlobalId = mobile.GlobalId,
                CustomerId = mobile.CustomerId,
                CreatedAt = mobile.CreatedAt,
                InFlightOrder = inFlightOrder,
                State = mobile.CurrentState.ToString(),
                OrderHistory = mobile.OrderHistory.Select(x => new OrderResource
                {
                    GlobalId = x.GlobalId,
                    State = x.CurrentState.ToString(),
                    Type = x.Type.ToString(),
                    CreatedAt = x.CreatedAt
                })
            });
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