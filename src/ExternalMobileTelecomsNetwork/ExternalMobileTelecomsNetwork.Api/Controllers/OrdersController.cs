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
                Reference = orderToAdd.Reference,
                Type = "Provision",
                Status = "New"
            };

            using (dataStore.BeginTransaction())
            {
                dataStore.Add(order);
            }

            return new OkResult();
        }

        [HttpGet("{reference}")]
        public ActionResult<Order> Get(Guid reference)
        {
            var order = dataStore.GetByReference(reference);

            if (order == null)
                return NotFound();

            return new OkObjectResult(order);
        }

        [HttpPost("{reference}/complete")]
        public IActionResult Complete(Guid reference)
        {
            var order = dataStore.GetByReference(reference);

            if (order == null)
                return NotFound();

            using (dataStore.BeginTransaction())
            {
                dataStore.Complete(reference);
            }

            return Ok();
        }

        [HttpDelete("{reference}")]
        public IActionResult Cancel(Guid reference)
        {
            var order = new Order
            {
                Reference = reference,
                Type = "Cancel",
                Status = "New"
            };

            using (dataStore.BeginTransaction())
            {
                dataStore.Add(order);
            }

            return new NoContentResult();
        }
    }
}