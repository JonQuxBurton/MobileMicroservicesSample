using System;
using Microsoft.Extensions.Logging;
using Mobiles.Api.Resources;
using Utils.DateTimes;
using Utils.DomainDrivenDesign;
using Utils.Guids;

namespace Mobiles.Api.Domain
{
    public class CustomersService : ICustomersService
    {
        private readonly ICustomerRepository customerRepository;
        private readonly IDateTimeCreator dateTimeCreator;
        private readonly IGuidCreator guidCreator;
        private readonly ILogger<CustomersService> logger;
        private readonly IRepository<Mobile> mobileRepository;

        public CustomersService(ILogger<CustomersService> logger,
            IGuidCreator guidCreator,
            IDateTimeCreator dateTimeCreator,
            ICustomerRepository customerRepository,
            IRepository<Mobile> mobileRepository)
        {
            this.logger = logger;
            this.guidCreator = guidCreator;
            this.dateTimeCreator = dateTimeCreator;
            this.customerRepository = customerRepository;
            this.mobileRepository = mobileRepository;
        }

        public Customer Create(CustomerToAdd customerToAdd)
        {
            var newCustomer = new Customer
            {
                GlobalId = guidCreator.Create(),
                Name = customerToAdd.Name
            };
            customerRepository.Add(newCustomer);

            var created = customerRepository.GetById(newCustomer.GlobalId);

            logger.LogInformation("Created Customer with GlobalId {GlobalId}", created.GlobalId);

            return created;
        }

        public Mobile Provision(Guid id, OrderToAdd orderToAdd)
        {
            var customer = customerRepository.GetById(id);

            if (customer == null)
            {
                logger.LogWarning("Attempt to Provision for an unknown Customer - CustomerId: {CustomerId}", id);
                return null;
            }

            var mobile = new MobileWhenNewBuilder(dateTimeCreator, guidCreator.Create(), id,
                    new PhoneNumber(orderToAdd.PhoneNumber))
                .AddInProgressOrder(orderToAdd, guidCreator.Create())
                .Build();
            mobileRepository.Add(mobile);

            return mobile;
        }
    }
}