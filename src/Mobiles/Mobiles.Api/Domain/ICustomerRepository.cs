using System.Collections.Immutable;
using Utils.DomainDrivenDesign;

namespace Mobiles.Api.Domain
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        IImmutableList<Customer> GetAll();
    }
}