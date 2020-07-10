using Microsoft.AspNetCore.Mvc;
using SimCardWholesaler.Api.Data;
using SimCardWholesaler.Api.Resources;
using System;

namespace SimCardWholesaler.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersDataStore ordersDataStore;

        public OrdersController(IOrdersDataStore ordersDataStore)
        {
            this.ordersDataStore = ordersDataStore;
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

            using (this.ordersDataStore.BeginTransaction())
            {
                this.ordersDataStore.Add(order);
            }

            return new OkResult();
        }

        [HttpGet("{reference}")]
        public IActionResult Get(Guid reference)
        {
            var order = this.ordersDataStore.GetByReference(reference);

            if (order == null)
                return NotFound();

            return new OkObjectResult(order);
        }

        [HttpPost("{reference}/complete")]
        public IActionResult Complete(Guid reference)
        {
            var order = this.ordersDataStore.GetByReference(reference);

            if (order == null)
                return NotFound();

            using (this.ordersDataStore.BeginTransaction())
            {
                this.ordersDataStore.Complete(order);
            }

            return new OkObjectResult(order);
        }
    }
}