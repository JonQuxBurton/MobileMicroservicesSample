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

        [HttpPost]
        public IActionResult Create([FromBody] OrderToAdd orderToAdd)
        {
            var order = new Order
            {
                PhoneNumber = orderToAdd.PhoneNumber,
                Reference = orderToAdd.Reference,
                Type = "Provision",
                Status = "New",
                ActivationCode = orderToAdd.ActivationCode
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

            if (order.Type == "Provision")
            {
                using (dataStore.BeginTransaction())
                {
                    var activationCode = dataStore.GetActivationCode(order.PhoneNumber);

                    if (activationCode.Code != order.ActivationCode)
                    {
                        var reason = "Invalid ActivationCode";
                        dataStore.Reject(order.Reference, reason);

                        return new BadRequestObjectResult(reason);
                    }
                }
            }

            using (dataStore.BeginTransaction())
            {
                dataStore.Complete(reference);
            }

            return Ok();
        }


        [HttpDelete("{phoneNumber}/{mobileReference}")]
        public IActionResult Cease(string phoneNumber, Guid mobileReference)
        {
            using (dataStore.BeginTransaction())
            {
                dataStore.Cease(phoneNumber, mobileReference);
            }

            return new NoContentResult();
        }
    }
}