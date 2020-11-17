using System;
using Mobiles.Api.Resources;

namespace Mobiles.Api.Domain
{
    public interface ICustomersService
    {
        Customer Create(CustomerToAdd customerToAdd);
        Mobile Provision(Guid id, OrderToAdd orderToAdd);
    }
}