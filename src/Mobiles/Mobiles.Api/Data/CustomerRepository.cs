using System;
using System.Collections.Immutable;
using System.Linq;
using Mobiles.Api.Domain;

namespace Mobiles.Api.Data
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly MobilesContext mobilesContext;

        public CustomerRepository(MobilesContext mobilesContext)
        {
            this.mobilesContext = mobilesContext;
        }

        public void Add(Customer customer)
        {
            mobilesContext.Customers.Add(customer);
            mobilesContext.SaveChanges();
        }

        public IImmutableList<Customer> GetAll()
        {
            return ImmutableList<Customer>.Empty.AddRange(mobilesContext.Customers);
        }

        public Customer GetById(Guid globalId)
        {
            var dbEntity = mobilesContext.Customers.FirstOrDefault(x => x.GlobalId == globalId);

            if (dbEntity == null)
                return null;

            return dbEntity;
        }

        public void Update(Customer aggregateRoot)
        {
            this.mobilesContext.SaveChanges();
        }
    }
}