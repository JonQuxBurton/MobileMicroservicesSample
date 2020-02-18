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
                Name = orderToAdd.Name,
                Reference = orderToAdd.Reference,
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

        [HttpPost("complete")]
        public IActionResult Complete([FromBody] OrderToComplete orderToComplete)
        {
            var order = dataStore.GetByReference(orderToComplete.Reference);

            if (order == null)
                return NotFound();

            using (dataStore.BeginTransaction())
            {
                dataStore.Complete(order);
            }

            return new OkObjectResult(order);
        }
    }
}