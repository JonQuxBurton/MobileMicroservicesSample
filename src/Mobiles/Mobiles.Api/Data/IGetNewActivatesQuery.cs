using Mobiles.Api.Domain;
using System.Collections.Generic;

namespace Mobiles.Api.Data
{
    public interface IGetNewActivatesQuery
    {
        IEnumerable<Mobile> Get();
    }
}