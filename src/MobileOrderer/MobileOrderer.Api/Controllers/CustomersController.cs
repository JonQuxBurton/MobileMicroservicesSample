using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MobileOrderer.Api.Domain;
using MobileOrderer.Api.Resources;
using System;
using Utils.Guids;

namespace MobileOrderer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ILogger<CustomersController> logger;
        private readonly ICustomerRepository repository;
        private readonly IMonitoring monitoring;
        private readonly IGuidCreator guidCreator;

        public CustomersController(ILogger<CustomersController> logger, ICustomerRepository repository, IMonitoring monitoring, IGuidCreator guidCreator)
        {
            this.logger = logger;
            this.repository = repository;
            this.monitoring = monitoring;
            this.guidCreator = guidCreator;
        }

        [HttpGet]
        public ActionResult<Customer> GetAll()
        {
            var entity = this.repository.GetAll();

            return new OkObjectResult(entity);
        }

        [HttpGet("{id}")]
        public ActionResult<Customer> Get(Guid id)
        {
            var entity = this.repository.GetById(id);

            if (entity == null)
                return NotFound();

            return new OkObjectResult(entity);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CustomerToAdd customerToAdd)
        {
            var newCustomer = new Customer
            {
                GlobalId = guidCreator.Create(),
                Name = customerToAdd.Name
            };
            repository.Add(newCustomer);

            var created = repository.GetById(newCustomer.GlobalId);

            logger.LogInformation("Created Customer with GlobalId {GlobalId}", created.GlobalId);
            monitoring.CreateCustomer();

            return new OkObjectResult(new CustomerResource
            {
                GlobalId = created.GlobalId,
                Nameof = created.Name,
                CreatedAt = created.CreatedAt,
                UpdatedAt = created.UpdatedAt
            });
        }
    }
}
