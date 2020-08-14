using System.Collections.Generic;
using Mobiles.Api.Domain;

namespace Mobiles.Api.Data
{
    public interface IGetNeProvisionsQuery
    {
        IEnumerable<Mobile> Get();
    }
}