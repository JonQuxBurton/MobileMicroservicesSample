using ExternalMobileTelecomsNetwork.Api.Data;
using ExternalMobileTelecomsNetwork.Api.Resources;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ExternalMobileTelecomsNetwork.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IDataStore dataStore;

        public OrdersController(IDataStore dataStore)
        {
            this.dataStore = dataStore;
        }

        [HttpGet("status")]
        public IActionResult Status()
        {
            return new OkResult();
        }

        [HttpPost]
        public IActionResult Create([FromBody] OrderToAdd orderToAdd)
        {
            var order = new Order
            {
                PhoneNumber = orderToAdd.PhoneNumber,
                MobileReference = orderToAdd.MobileReference,
                Type = "Provision",
                Status = "New"
            };

            using (dataStore.BeginTransaction())
            {
                dataStore.Add(order);
            }

            return new OkResult();
        }

        [HttpGet("{mobileReference}")]
        public ActionResult<Order> Get(Guid mobileReference)
        {
            var order = dataStore.GetByReference(mobileReference);

            if (order == null)
                return NotFound();

            return new OkObjectResult(order);
        }

        [HttpPost("{mobileReference}/complete")]
        public IActionResult Complete(Guid mobileReference)
        {
            var order = dataStore.GetByReference(mobileReference);

            if (order == null)
                return NotFound();

            using (dataStore.BeginTransaction())
            {
                dataStore.Complete(mobileReference);
            }

            return Ok();
        }

        [HttpDelete("{phoneNumber}/{mobileReference}")]
        public IActionResult Cease(string phoneNumber, Guid mobileReference)
        {
            using (dataStore.BeginTransaction())
            {
                dataStore.Cease(mobileReference, phoneNumber);
            }

            return new NoContentResult();
        }
    }
}