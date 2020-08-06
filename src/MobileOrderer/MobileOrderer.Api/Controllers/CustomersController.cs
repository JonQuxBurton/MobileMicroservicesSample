using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MobileOrderer.Api.Domain;
using MobileOrderer.Api.Resources;
using System;
using System.Linq;
using Utils.DomainDrivenDesign;
using Utils.Guids;

namespace MobileOrderer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ILogger<CustomersController> logger;
        private readonly ICustomerRepository repository;
        private readonly IRepository<Mobile> mobileRepository;
        private readonly IMonitoring monitoring;
        private readonly IGuidCreator guidCreator;

        public CustomersController(
            ILogger<CustomersController> logger,
            ICustomerRepository repository,
            IRepository<Mobile> mobileRepository,
            IMonitoring monitoring,
            IGuidCreator guidCreator)
        {
            this.logger = logger;
            this.repository = repository;
            this.mobileRepository = mobileRepository;
            this.monitoring = monitoring;
            this.guidCreator = guidCreator;
        }

        [HttpGet]
        public ActionResult<Customer[]> GetAll()
        {
            var entities = this.repository.GetAll();

            return new OkObjectResult(entities.ToArray());
        }

        [HttpGet("{id}")]
        public ActionResult<Customer> Get(Guid id)
        {
            var entity = repository.GetById(id);

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
                Name = created.Name,
                CreatedAt = created.CreatedAt,
                UpdatedAt = created.UpdatedAt
            });
        }

        [HttpPost("{id}/provision")]
        public IActionResult Provision(Guid id, [FromBody] OrderToAdd orderToAdd)
        {
            var customer = repository.GetById(id);

            if (customer == null)
            {
                logger.LogWarning("Attempt to Provision for an unknown Customer - CustomerId: {CustomerId}", id);
                return NotFound();
            }

            var mobile = new MobileWhenNewBuilder(this.guidCreator.Create(), id)
                            .AddInFlightOrder(orderToAdd, this.guidCreator.Create())
                            .Build();
            mobileRepository.Add(mobile);

            monitoring.Provision();

            return new OkObjectResult(new MobileResource
            {
                Id = mobile.Id,
                GlobalId = mobile.GlobalId,
                CustomerId = mobile.CustomerId,
                CreatedAt = mobile.CreatedAt,
                UpdatedAt = mobile.UpdatedAt
            });
        }
    }
}
