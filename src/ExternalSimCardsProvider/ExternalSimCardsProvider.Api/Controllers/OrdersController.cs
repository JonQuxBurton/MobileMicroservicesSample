using Microsoft.AspNetCore.Mvc;
using ExternalSimCardsProvider.Api.Data;
using ExternalSimCardsProvider.Api.Resources;
using System;
using ExternalSimCardsProvider.Api.Domain;

namespace ExternalSimCardsProvider.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersDataStore ordersDataStore;
        private readonly IActivationCodeGenerator activationCodeGenerator;

        public OrdersController(IOrdersDataStore ordersDataStore, IActivationCodeGenerator activationCodeGenerator)
        {
            this.ordersDataStore = ordersDataStore;
            this.activationCodeGenerator = activationCodeGenerator;
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
                ordersDataStore.Add(order);
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
                order.ActivationCode = activationCodeGenerator.Generate();
                this.ordersDataStore.Complete(order);
            }

            return new OkObjectResult(order);
        }

        [HttpGet("{reference}/activationcode")]
        public IActionResult GetActivationCode(Guid reference)
        {
            var order = this.ordersDataStore.GetByReference(reference);

            if (order == null)
                return NotFound();

            if (order.ActivationCode == null)
                return NoContent();

            return new OkObjectResult(order.ActivationCode);
        }
    }
}