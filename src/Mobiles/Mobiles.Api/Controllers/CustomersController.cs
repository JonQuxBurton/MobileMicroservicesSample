using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Mobiles.Api.Data;
using Mobiles.Api.Domain;
using Mobiles.Api.Resources;
using System;
using System.Linq;
using Utils.DomainDrivenDesign;
using Utils.Guids;

namespace Mobiles.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ILogger<CustomersController> logger;
        private readonly ICustomerRepository customerRepository;
        private readonly IRepository<Mobile> mobileRepository;
        private readonly IMonitoring monitoring;
        private readonly IGuidCreator guidCreator;
        private readonly IGetMobilesByCustomerIdQuery getMobilesByCustomerIdQuery;

        public CustomersController(
            ILogger<CustomersController> logger,
            ICustomerRepository customerRepository,
            IRepository<Mobile> mobileRepository,
            IMonitoring monitoring,
            IGuidCreator guidCreator,
            IGetMobilesByCustomerIdQuery getMobilesByCustomerIdQuery)
        {
            this.logger = logger;
            this.customerRepository = customerRepository;
            this.mobileRepository = mobileRepository;
            this.monitoring = monitoring;
            this.guidCreator = guidCreator;
            this.getMobilesByCustomerIdQuery = getMobilesByCustomerIdQuery;
        }

        [HttpGet]
        public ActionResult<Customer[]> GetAll()
        {
            var entities = this.customerRepository.GetAll();

            return new OkObjectResult(entities.ToArray());
        }

        [HttpGet("{id}")]
        public ActionResult<Customer> Get(Guid id)
        {
            var customer = customerRepository.GetById(id);

            if (customer == null)
                return NotFound();

            var mobiles = getMobilesByCustomerIdQuery.Get(id);

            return new OkObjectResult(
                new CustomerResource
                {
                    Name = customer.Name,
                    GlobalId = customer.GlobalId,
                    CreatedAt = customer.CreatedAt,
                    Mobiles = mobiles.Select(x =>
                    {
                        var mobile = mobileRepository.GetById(x.GlobalId);

                        OrderResource inFlightOrder = null;

                        if (mobile.InFlightOrder != null)
                        {
                            inFlightOrder = new OrderResource
                            {
                                State = mobile.InFlightOrder.CurrentState.ToString(),
                                //Type = mobile.InFlightOrder.Type.ToString(),
                                //CreatedAt = mobile.InFlightOrder.CreatedAt,
                                //ActivationCode = mobile.InFlightOrder.ActivationCode
                            };
                        }

                        return new MobileResource
                        {
                            GlobalId = x.GlobalId,
                            CreatedAt = x.CreatedAt,
                            CustomerId = x.CustomerId,
                            PhoneNumber = x.PhoneNumber.ToString(),
                            State = x.CurrentState.ToString(),
                            InFlightOrder = inFlightOrder
                        };
                    }).ToArray()
                });
        }

        [HttpPost]
        public IActionResult Create([FromBody] CustomerToAdd customerToAdd)
        {
            var newCustomer = new Customer
            {
                GlobalId = guidCreator.Create(),
                Name = customerToAdd.Name
            };
            customerRepository.Add(newCustomer);

            var created = customerRepository.GetById(newCustomer.GlobalId);

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
            var customer = customerRepository.GetById(id);

            if (customer == null)
            {
                logger.LogWarning("Attempt to Provision for an unknown Customer - CustomerId: {CustomerId}", id);
                return NotFound();
            }

            var mobile = new MobileWhenNewBuilder(this.guidCreator.Create(), id, new PhoneNumber(orderToAdd.PhoneNumber))
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
