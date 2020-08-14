using System.Collections.Generic;
using Mobiles.Api.Domain;

namespace Mobiles.Api.Data
{
    public interface IGetNewCeasesQuery
    {
        IEnumerable<Mobile> Get();
    }
}