using System.Collections.Immutable;
using Utils.DomainDrivenDesign;

namespace MobileOrderer.Api.Domain
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        IImmutableList<Customer> GetAll();
    }
}