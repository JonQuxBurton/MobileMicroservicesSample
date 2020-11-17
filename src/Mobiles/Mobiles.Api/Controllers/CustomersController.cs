using Microsoft.AspNetCore.Mvc;
using Mobiles.Api.Data;
using Mobiles.Api.Domain;
using Mobiles.Api.Resources;
using System;
using System.Linq;
using Utils.DomainDrivenDesign;

namespace Mobiles.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerRepository customerRepository;
        private readonly IRepository<Mobile> mobileRepository;
        private readonly IMonitoring monitoring;
        private readonly IGetMobilesByCustomerIdQuery getMobilesByCustomerIdQuery;
        private readonly ICustomersService customersService;

        public CustomersController(ICustomerRepository customerRepository,
            IRepository<Mobile> mobileRepository,
            IMonitoring monitoring,
            IGetMobilesByCustomerIdQuery getMobilesByCustomerIdQuery,
            ICustomersService customersService)
        {
            this.customerRepository = customerRepository;
            this.mobileRepository = mobileRepository;
            this.monitoring = monitoring;
            this.getMobilesByCustomerIdQuery = getMobilesByCustomerIdQuery;
            this.customersService = customersService;
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

                        return new MobileResource
                        {
                            GlobalId = x.GlobalId,
                            CreatedAt = x.CreatedAt,
                            CustomerId = x.CustomerId,
                            PhoneNumber = x.PhoneNumber.ToString(),
                            State = x.State.ToString(),
                            Orders = mobile.Orders.Select(order => new OrderResource
                            {
                                GlobalId = order.GlobalId,
                                State = order.CurrentState.ToString(),
                                Type = order.Type.ToString(),
                                CreatedAt = order.CreatedAt,
                                ActivationCode = order.ActivationCode
                            })
                        };
                    }).ToArray()
                });
        }

        [HttpPost]
        public IActionResult Create([FromBody] CustomerToAdd customerToAdd)
        {
            var created = customersService.Create(customerToAdd);
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
            var mobile = customersService.Provision(id, orderToAdd);

            if (mobile == null)
                return NotFound();

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
