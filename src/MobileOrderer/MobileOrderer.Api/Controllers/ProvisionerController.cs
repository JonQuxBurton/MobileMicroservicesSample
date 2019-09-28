using Microsoft.AspNetCore.Mvc;
using MobileOrderer.Api.Data;
using MobileOrderer.Api.Resources;
using System;

namespace MobileOrderer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProvisionerController : ControllerBase
    {
        private readonly IOrdersDataStore orderDataStore;

        public ProvisionerController(IOrdersDataStore orderDataStore)
        {
            this.orderDataStore = orderDataStore;
        }

        [HttpGet("status")]
        public IActionResult Status()
        {
            return new OkResult();
        }

        [HttpPost]
        public IActionResult Post([FromBody] MobileOrderToAdd orderToAdd)
        {
            var order = new MobileOrder(orderToAdd)
            {
                GlobalId = Guid.NewGuid(),
                Status = "New"
            };

            using (this.orderDataStore.BeginTransaction())
            {
                this.orderDataStore.Add(order);
            }

            return Ok();
        }
    }
}