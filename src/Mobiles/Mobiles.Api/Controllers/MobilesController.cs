using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Mobiles.Api.Data;
using Mobiles.Api.Domain;
using Mobiles.Api.Resources;
using Utils.DomainDrivenDesign;
using Utils.Guids;

namespace Mobiles.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MobilesController : ControllerBase
    {
        private readonly IGetNextMobileIdQuery getNextMobileIdQuery;
        private readonly IGuidCreator guidCreator;
        private readonly ILogger<MobilesController> logger;
        private readonly IRepository<Mobile> mobileRepository;
        private readonly IMobilesService mobilesService;
        private readonly IMonitoring monitoring;

        public MobilesController(ILogger<MobilesController> logger, IMobilesService mobilesService,
            IRepository<Mobile> mobileRepository, IGuidCreator guidCreator, IMonitoring monitoring,
            IGetNextMobileIdQuery getNextMobileIdQuery)
        {
            this.logger = logger;
            this.mobilesService = mobilesService;
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
                PhoneNumbers = new[] {$"07{nextId}000{nextId}"}
            };
        }

        [HttpGet("{id}")]
        public ActionResult<Mobile> Get(Guid id)
        {
            var mobile = mobileRepository.GetById(id);

            if (mobile == null)
                return NotFound();

            return new OkObjectResult(new MobileResource
            {
                Id = mobile.Id,
                GlobalId = mobile.GlobalId,
                CustomerId = mobile.CustomerId,
                CreatedAt = mobile.CreatedAt,
                State = mobile.State.ToString(),
                Orders = mobile.Orders.Select(x => new OrderResource
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
            var mobile = mobilesService.Activate(id, activateRequest);

            if (mobile == null)
                return NotFound();

            monitoring.Activate();

            var newOrder = mobile.InProgressOrder;

            return new OkObjectResult(new OrderResource
            {
                GlobalId = newOrder.GlobalId,
                Name = newOrder.Name,
                ContactPhoneNumber = newOrder.ContactPhoneNumber,
                State = newOrder.CurrentState.ToString(),
                Type = newOrder.Type.ToString(),
                CreatedAt = newOrder.CreatedAt,
                UpdatedAt = newOrder.UpdatedAt
            });
        }

        [HttpDelete("{id}")]
        public IActionResult Cease(Guid id)
        {
            var mobile = mobilesService.Cease(id);

            if (mobile == null) return NotFound();

            monitoring.Cease();

            return Accepted();
        }
    }
}