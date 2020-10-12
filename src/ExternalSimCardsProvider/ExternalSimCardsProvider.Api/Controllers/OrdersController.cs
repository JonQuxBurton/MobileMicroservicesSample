using Microsoft.AspNetCore.Mvc;
using ExternalSimCardsProvider.Api.Data;
using ExternalSimCardsProvider.Api.Resources;
using System;
using ExternalSimCardsProvider.Api.Domain;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ExternalSimCardsProvider.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersDataStore ordersDataStore;
        private readonly IActivationCodeGenerator activationCodeGenerator;
        private readonly IExternalMobileTelecomsNetworkService externalMobileTelecomsNetworkService;

        public OrdersController(IOrdersDataStore ordersDataStore, 
            IActivationCodeGenerator activationCodeGenerator,
            IExternalMobileTelecomsNetworkService externalMobileTelecomsNetworkService)
        {
            this.ordersDataStore = ordersDataStore;
            this.activationCodeGenerator = activationCodeGenerator;
            this.externalMobileTelecomsNetworkService = externalMobileTelecomsNetworkService;
        }

        [HttpPost]
        public IActionResult Create([FromBody] OrderToAdd orderToAdd)
        {
            var order = new Order
            {
                PhoneNumber = orderToAdd.PhoneNumber,
                Name = orderToAdd.Name,
                Reference = orderToAdd.Reference,
                Status = "New"
            };

            using (ordersDataStore.BeginTransaction())
            {
                ordersDataStore.Add(order);
            }

            return new OkResult();
        }

        [HttpGet("{reference}")]
        public IActionResult Get(Guid reference)
        {
            var order = ordersDataStore.GetByReference(reference);

            if (order == null)
                return NotFound();

            return new OkObjectResult(order);
        }
        
        [HttpGet]
        public IActionResult GetAll()
        {
            var orders = ordersDataStore.GetAll();

            return new OkObjectResult(orders);
        }

        [HttpPost("{reference}/complete")]
        public async Task<IActionResult> Complete(Guid reference)
        {
            var order = this.ordersDataStore.GetByReference(reference);

            if (order == null)
                return NotFound();

            var activationCode = activationCodeGenerator.Generate();

            var result = await externalMobileTelecomsNetworkService.PostActivationCode(order.PhoneNumber, activationCode);

            if (!result)
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);

            using (ordersDataStore.BeginTransaction())
            {
                order.ActivationCode = activationCode;
                ordersDataStore.Complete(order);
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

            return new OkObjectResult(new ActivationCodeResource { ActivationCode = order.ActivationCode });
        }
    }
}